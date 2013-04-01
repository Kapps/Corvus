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

namespace CorvEngine.Components
{
    public enum ArenaPhases
    {
        Start, Battle, End
    }

    public class ArenaSystem : SceneSystem
    {
        private const float PHASE_DURATION = 5;

        private int _Wave = 1;
        private int _TotalEntitiesRemaining = 0;
        private int _TotalEntitiesKilled = 0;
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

        public int Wave
        {
            get { return _Wave; }
            set { _Wave = value; }
        }

        public int TotalEntitiesKilled
        {
            get { return _TotalEntitiesKilled; }
            set { _TotalEntitiesKilled = value; }
        }

        public void Reset()
        {
            Wave = 1;
            _DifficultyModifier = 1f;
            _CurrentPhase = ArenaPhases.Start;
            RemainingEntities = new List<Entity>();
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
                if(!sc.IsOnEnemySpawnRegistered) //Becareful with this. Not sure how often OnInitialized is called.
                    sc.OnEnemySpawn += sc_OnEnemySpawn;
            }

            Reset();
        }

        protected override void OnUpdate(GameTime Time)
        {
            //Not sure if it's a bug, but this is called multiple times before gametime is even determined.
            if (Time.TotalGameTime == TimeSpan.Zero)
                return;

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
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();
           

            if (_CurrentPhase == ArenaPhases.Start)
                CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Battle starts in {0}...", _PhaseTimer.Seconds.ToString()), new Vector2(0f, 200f), Color.Yellow);
            else if (_CurrentPhase == ArenaPhases.Battle)
            {
                CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Enemies Remaining: {0}", _TotalEntitiesRemaining.ToString()), new Vector2(0f, 200f), Color.Yellow);
                string battleTime = string.Format("{0}.{1}s", _BattleTimer.Seconds.ToString(), _BattleTimer.Milliseconds.ToString());
                CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Time: {0}", battleTime), new Vector2(0f, 240f), Color.Yellow);
            }
            else
            {
                string battleTime = string.Format("{0}.{1}s", _BattleTimer.Seconds.ToString(), _BattleTimer.Milliseconds.ToString());
                CorvusGame.Instance.SpriteBatch.DrawString(_Font, string.Format("Cleared Wave {0} in {1}!", Wave.ToString(), battleTime), new Vector2(0f, 200f), Color.Yellow);
            }
        }

        void sc_OnEnemySpawn(object sender, EnemySpawnedEvent e)
        {
            RemainingEntities.Add(e.Entity);
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

            _TotalEntitiesRemaining = Wave * EnemySpawners.Count + Rand.Next(Wave / 4, Wave / 2);
            foreach (var s in EnemySpawners)
            {
                var sc = s.GetComponent<SpawnerComponent>();
                sc.TotalEntitiesToSpawn = _TotalEntitiesRemaining / EnemySpawners.Count;
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

            Wave++;

            //create healing items.
            foreach (var s in ItemSpawners)
                s.GetComponent<SpawnerComponent>().Spawn();

            //Set difficulty based on how long it took the player.
            _DifficultyModifier += 0.05f + (MathHelper.Clamp(1f / (float)_BattleTimer.TotalSeconds, 0f, 0.1f));
            foreach (var s in EnemySpawners)
            {
                var sc = s.GetComponent<SpawnerComponent>();
                sc.Reset();
                sc.DifficultyModifier = _DifficultyModifier;
            }
            
        }

        private void InitStartPhase()
        {
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
