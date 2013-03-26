using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Corvus.Components;

namespace CorvEngine.Components
{
    public class ArenaSystem : SceneSystem
    {
        public int Wave
        {
            get { return _Wave; }
        }

        private int _Wave;
        private int _TotalEntitiesSpawned;
        private int _TotalEntitiesSpawnedWave;

        //Unsure about these being here, but fine for now.
        private int _TotalEntitiesKilled;
        private int _TotalEntitiesKilledWave;

        protected override void OnUpdate(GameTime Time)
        {
            foreach (Entity entity in Scene.Entities.Where(c => c.GetComponent<SpawnerComponent>() != null))
            {
                
            }
        }
    }
}
