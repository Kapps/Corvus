using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components;
using Corvus;
using Corvus.Interface;

namespace CorvEngine.Components
{
    public enum ArenaPhases
    {
        Start, Battle, End, GameOver
    }

    public class ArenaSystem : SceneSystem
    {
        private const float PHASE_DURATION = 5f;
        private const float GAMEOVER_PHASE_DURATION = 10f;
        private List<string> EnemyCollection = new List<string>() { 
            "GoombaGolbez", "Golbez", "DarkKnight", "Zemus", "CecilHarvey", "PoisonCecilHarvey", "KainHighwind", "WorkoutKainHighwind"
        };

        //stats
        private int _Wave = 1;
        private int _TotalEntitiesRemaining = 0;
        private int _TotalEntitiesKilled = 0;
        private TimeSpan _TotalTime = TimeSpan.Zero;
        private float _DifficultyModifier = 1f;

        private ArenaPhases _CurrentPhase = ArenaPhases.Start;
        private TimeSpan _PhaseTimer = TimeSpan.FromSeconds(PHASE_DURATION);
        private TimeSpan _BattleTimer = TimeSpan.Zero;

        private SpriteFont _Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/ArenaFont");
        private Random Rand = new Random();

        private List<Entity> EnemySpawners;
        private List<Entity> WeaponSpawners;
        private List<Entity> ItemSpawners;

        private List<Entity> RemainingEntities;
        
        public void Reset()
        {
            _Wave = 1;
            _TotalTime = TimeSpan.Zero;
            _TotalEntitiesKilled = 0;
            _DifficultyModifier = 1f;
            _CurrentPhase = ArenaPhases.Start;

            if (RemainingEntities != null)
            {
                foreach (var s in RemainingEntities)
                {
                    if(!s.IsDisposed)
                        s.Dispose();
                }
            }
            RemainingEntities = new List<Entity>();
            _BattleTimer = TimeSpan.Zero;
            _PhaseTimer = TimeSpan.FromSeconds(PHASE_DURATION);
            foreach (var s in EnemySpawners)
                s.GetComponent<SpawnerComponent>().Reset();
        }

        protected override void OnInitialize()
        {
            WeaponSpawners = Scene.Entities.Where(c => c.GetComponent<SpawnerComponent>() != null && c.GetComponent<SpawnerComponent>().SpawnID == "Weapon").ToList();
            EnemySpawners = Scene.Entities.Where(c => c.GetComponent<SpawnerComponent>() != null && c.GetComponent<SpawnerComponent>().SpawnID == "Enemy").ToList();
            ItemSpawners = Scene.Entities.Where(c => c.GetComponent<SpawnerComponent>() != null && c.GetComponent<SpawnerComponent>().SpawnID == "Item").ToList();

            foreach (var s in WeaponSpawners.Concat(EnemySpawners).Concat(ItemSpawners))
            {
                var sc = s.GetComponent<SpawnerComponent>();
                sc.SpawnerEnabled = false;
                if(!sc.IsOnEnemySpawnRegistered) 
                    sc.OnEnemySpawn += sc_OnEnemySpawn;
                if (sc.SpawnID == "Enemy")
                    sc.EntitiesToSpawn = EnemyCollection;
                else {
                    sc.SpawnRangeMin = 0;
                    sc.SpawnRangeMax = sc.EntitiesToSpawn.Count;
                }
            }
            
            Reset();
        }

        protected override void OnUpdate(GameTime Time)
        {
            //Not sure if it's a bug, but this is called multiple times before gametime is even determined.
            if (Time.TotalGameTime == TimeSpan.Zero)
                return;

            DeterminePlayerState();
            switch (_CurrentPhase)
            {
                case ArenaPhases.Start:
                    StartPhaseUpdate();
                    _PhaseTimer -= Time.ElapsedGameTime;
                    if (_PhaseTimer <= TimeSpan.Zero)
                    {
                        AudioManager.PlaySoundEffect("ArenaStart");
                        InitBattlePhase();
                        _BattleTimer = TimeSpan.Zero;
                        _PhaseTimer = TimeSpan.FromSeconds(PHASE_DURATION);
                        _CurrentPhase = ArenaPhases.Battle;
                    }
                    break;
                case ArenaPhases.Battle:
                    BattlePhaseUpdate();
                    _BattleTimer += Time.ElapsedGameTime;
                    if (RemainingEntities.Count == 0)
                    {
                        AudioManager.PlaySoundEffect("ArenaEnd");
                        InitEndPhase();
                        _PhaseTimer = TimeSpan.FromSeconds(PHASE_DURATION);
                        _CurrentPhase = ArenaPhases.End;
                    }
                    break;
                case ArenaPhases.End:
                    EndPhaseUpdate();
                    _PhaseTimer -= Time.ElapsedGameTime;
                    if (_PhaseTimer <= TimeSpan.Zero)
                    {
                        InitStartPhase();
                        _PhaseTimer = TimeSpan.FromSeconds(PHASE_DURATION);
                        _CurrentPhase = ArenaPhases.Start;
                    }
                    break;
                case ArenaPhases.GameOver:
                    _PhaseTimer -= Time.ElapsedGameTime;
                    if (_PhaseTimer <= TimeSpan.Zero)
                    {
                        Reset();
                        _PhaseTimer = TimeSpan.FromSeconds(PHASE_DURATION);
                        _CurrentPhase = ArenaPhases.Start;
                        CorvusGame.Instance.SceneManager.ReloadScenes(true);
                    }
                    break;
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            if (_CurrentPhase != ArenaPhases.GameOver)
            {
                CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Wave: {0}", _Wave.ToString()), new Vector2(5f, 125f), Color.Black);
                CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Kills: {0}", _TotalEntitiesKilled), new Vector2(5f, 150f), Color.Black);
            }
            string battleTime = string.Format("{0}s", _BattleTimer.TotalSeconds.ToString("0.##"));
            switch (_CurrentPhase)
            {
                case ArenaPhases.Start:
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Battle starts in {0}...", (_PhaseTimer.Seconds + 1).ToString()), new Vector2(5f, 175f), Color.Yellow);
                    break;
                case ArenaPhases.Battle:
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Enemies Remaining: {0}", _TotalEntitiesRemaining.ToString()), new Vector2(5f, 175f), Color.Yellow);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Time: {0}", battleTime), new Vector2(5f, 200f), Color.Yellow);
                    break;
                case ArenaPhases.End:    
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Cleared Wave {0} in {1}!", _Wave.ToString(), battleTime), new Vector2(5f, 175f), Color.Yellow);
                    break;
                case ArenaPhases.GameOver:
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, "You Have Died", new Vector2(5f, 125f), Color.Yellow);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, "Stats", new Vector2(5f, 150f), Color.Black);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Waves Cleared: {0}", (_Wave - 1).ToString()), new Vector2(15f, 175f), Color.Black);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Total Kills: {0}", _TotalEntitiesKilled), new Vector2(15f, 200f), Color.Black);
                    string totalTime = string.Format("{0}s", _TotalTime.TotalSeconds.ToString("0.##"));
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Total Time: {0}", totalTime), new Vector2(15f, 225f), Color.Black);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Difficulty Modifier: {0}", _DifficultyModifier.ToString("0.00")), new Vector2(15f, 250f), Color.Black);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Score: {0}", ((_Wave - 1) * _TotalEntitiesKilled * _DifficultyModifier).ToString("0")), new Vector2(15f, 275f), Color.Gold);
                    CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Restarting in {0}...", (_PhaseTimer.Seconds + 1).ToString()), new Vector2(5f, 300f), Color.Yellow);
                    break;
            }
        }

        void sc_OnEnemySpawn(object sender, EnemySpawnedEvent e)
        {
            RemainingEntities.Add(e.Entity);
        }

        private void DeterminePlayerState()
        {
            if (_CurrentPhase == ArenaPhases.GameOver)
                return;

            bool allplayersDisposed = true;
            foreach (var p in CorvusGame.Instance.Players)
            {
                if (!p.Character.IsDisposed)
                {
                    allplayersDisposed = false;
                    break;
                }
            }
            if (allplayersDisposed)
            {
                _TotalTime += _BattleTimer;
                _CurrentPhase = ArenaPhases.GameOver;
                _PhaseTimer = TimeSpan.FromSeconds(GAMEOVER_PHASE_DURATION);
            }

        }

        private void StartPhaseUpdate()
        {
            if (_PhaseTimer < TimeSpan.FromSeconds(PHASE_DURATION))
                return;

            foreach (var s in WeaponSpawners)
                s.GetComponent<SpawnerComponent>().Spawn();
        }

        private void InitBattlePhase()
        {
            //Remove existing weapons.
            foreach (var e in Scene.Entities.Where(en => en.GetComponent<WeaponPropertiesComponent>() != null).ToList())
                e.Dispose();
            RemainingEntities = new List<Entity>();

            _TotalEntitiesRemaining = _Wave * EnemySpawners.Count + Rand.Next(_Wave / 4, _Wave / 2);
            foreach (var s in EnemySpawners)
            {
                var sc = s.GetComponent<SpawnerComponent>();
                sc.TotalEntitiesToSpawn = _TotalEntitiesRemaining / EnemySpawners.Count;
                sc.SpawnDelay = MathHelper.Clamp(3f - _DifficultyModifier , 1f, 2f);
                sc.SpawnerEnabled = true;
            }
        }
        
        private void BattlePhaseUpdate()
        {
            foreach (var e in RemainingEntities.Reverse<Entity>())
            {
                if (e.IsDisposed)
                {
                    _TotalEntitiesKilled++;
                    _TotalEntitiesRemaining--;
                    RemainingEntities.Remove(e);
                }
            }
        }

        private void InitEndPhase()
        {

        }

        private void EndPhaseUpdate()
        {
            if (_PhaseTimer < TimeSpan.FromSeconds(PHASE_DURATION))
                return;
            _TotalTime += _BattleTimer;
            //create healing items.
            foreach (var s in ItemSpawners)
                s.GetComponent<SpawnerComponent>().Spawn();            
        }

        private void InitStartPhase()
        {
            _Wave++;

            //Set difficulty based on how long it took the player.
            _DifficultyModifier += 0.025f + (MathHelper.Clamp(1f / (float)_BattleTimer.TotalSeconds, 0f, 0.1f));
            foreach (var s in EnemySpawners)
            {
                var sc = s.GetComponent<SpawnerComponent>();
                sc.Reset();
                sc.DifficultyModifier = _DifficultyModifier;
                sc.SpawnRangeMin = _Wave / 4;
                sc.SpawnRangeMax = _Wave / 2;
            }

            //remove all existing items.
            foreach (var e in RemainingEntities)
            {
                if (Scene.Entities.Contains(e))
                    e.Dispose();
            }
            RemainingEntities = new List<Entity>();
        }
    }
}
