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

        //Best to put this somewhere else?
        public int EntitiesToSpawn
        {
            get { return _EntitiesToSpawn; }
            set { _EntitiesToSpawn = value; }
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

        private bool _SpawnerEnabled = true;
        private DateTime _LastSpawn;
        private List<Entity> _EntitiesSpawned = new List<Entity>();
        private int _TotalEntitiesSpawned;
        private int _EntitiesToSpawn = 1;
        private ArenaSystem ArenaSystem;

        protected override void OnUpdate(GameTime Time)
        {
            if ((DateTime.Now - LastSpawn).TotalSeconds > 2 && TotalEntitiesSpawned != EntitiesToSpawn && SpawnerEnabled)
            {
                Entity entity = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("TestEntityEnemy").CreateEntity();
                entity.Position = this.Parent.Position;
                entity.Size = new Vector2(32, 32);
                Scene.AddEntity(entity);
                TotalEntitiesSpawned++;
                EntitiesSpawned.Add(entity);
                LastSpawn = DateTime.Now;
            }

            base.OnUpdate(Time);
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
