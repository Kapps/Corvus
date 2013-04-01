using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;
using CorvEngine;

namespace Corvus.Components
{
    public class SpawnerComponent : Component
    {
        public DateTime LastSpawn
        {
            get { return _LastSpawn; }
            set { _LastSpawn = value; }
        }

        public int TotalEntitiesSpawned
        {
            get { return _TotalEntitiesSpawned; }
            set { _TotalEntitiesSpawned = value; }
        }

        public int TotalEntitiesToSpawn
        {
            get { return _TotalEntitiesToSpawn; }
            set { _TotalEntitiesToSpawn = value; }
        }

        public bool SpawnerEnabled
        {
            get { return _SpawnerEnabled; }
            set { _SpawnerEnabled = value; }
        }

        public List<Entity> EntitiesSpawned
        {
            get { return _EntitiesSpawned; }
            set { _EntitiesSpawned = value; }
        }

        public string BlueprintName
        {
            get { return _BlueprintName; }
            set { _BlueprintName = value; }
        }

        public float SpawnDelay
        {
            get { return _SpawnDelay; }
            set { _SpawnDelay = value; }
        }

        private bool _SpawnerEnabled = true;
        private DateTime _LastSpawn;
        private List<Entity> _EntitiesSpawned = new List<Entity>();
        private int _TotalEntitiesSpawned;
        private int _TotalEntitiesToSpawn;
        private ArenaSystem ArenaSystem;
        private string _BlueprintName;
        private float _SpawnDelay;

        protected override void OnUpdate(GameTime Time)
        {
            if ((DateTime.Now - LastSpawn).TotalMilliseconds > SpawnDelay && TotalEntitiesSpawned != TotalEntitiesToSpawn && SpawnerEnabled)
            {
                //Set up the basic entity.
                Entity entity = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(BlueprintName).CreateEntity();
                entity.Position = this.Parent.Position;
                entity.Size = new Vector2(32, 32);

                //Modify the entity's difficulty, based on the wave.
                //Was debating setting the modifiers, but this just seems to make sense, as they're a completely new entity.
                var ac = entity.GetComponent<AttributesComponent>();
                var difficulty = GetDifficulty();
                ac.Strength = ac.Strength * difficulty;
                ac.MaxHealth = ac.MaxHealth * difficulty;

                //Add the entity to the game.
                Scene.AddEntity(entity);
                TotalEntitiesSpawned++;
                EntitiesSpawned.Add(entity);
                LastSpawn = DateTime.Now;
            }

            base.OnUpdate(Time);
        }

        //Make this a property if we have more complicated spawners eventually, as we'll probably set it in tiled.
        private float GetDifficulty()
        {
            if (ArenaSystem == null)
                return 1;
            else
                return ArenaSystem.Wave / 10f;
        }

        protected override void OnInitialize()
        {
            ArenaSystem = Scene.GetSystem<ArenaSystem>();
        }

        public void Reset()
        {
            EntitiesSpawned = new List<Entity>();
            _TotalEntitiesSpawned = 0;
        }
    }
}
