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

        private DateTime _LastSpawn;
        private List<Entity> _SpawnedEntities;
        private bool _Enabled;

        protected override void OnUpdate(GameTime Time)
        {
            if ((DateTime.Now - LastSpawn).TotalSeconds > 2)
            {
                Entity entity = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("TestEntityEnemy").CreateEntity();
                entity.Position = this.Parent.Position;
                entity.Size = new Vector2(32, 32);
                Scene.AddEntity(entity);
                LastSpawn = DateTime.Now;
            }

            base.OnUpdate(Time);
        }

        protected override void OnInitialize()
        {

        }
    }
}
