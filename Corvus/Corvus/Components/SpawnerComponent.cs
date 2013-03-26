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
        private DateTime _LastSpawn;
        private List<Entity> _SpawnedEntities;

        /*
        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
        }

        protected override void OnInitialize()
        {

        }
        */
    }
}
