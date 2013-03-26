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
            set { _Wave = value; }
        }

        private int _Wave = 1;
        private int _TotalEntitiesSpawned;
        private int _TotalEntitiesSpawnedWave;

        //Unsure about these being here, but fine for now.
        private int _TotalEntitiesKilled;
        private int _TotalEntitiesKilledWave;

        private List<Entity> SpawnerEntities;
        private List<SpawnerComponent> SpawnerComponents = new List<SpawnerComponent>();

        protected override void OnUpdate(GameTime Time)
        {
            bool allEntitiesDisposed = true;

            foreach (SpawnerComponent spawnerComponent in SpawnerComponents)
            {
                foreach (Entity entity in spawnerComponent.EntitiesSpawned)
                {
                    if (entity.IsDisposed == false)
                        allEntitiesDisposed = false;
                }
            }

            if (allEntitiesDisposed)
            {
                foreach (SpawnerComponent spawnerComponent in SpawnerComponents)
                {
                    spawnerComponent.Reset();
                }

                Wave++;
            }
        }

        protected override void OnInitialize()
        {
            SpawnerEntities = Scene.Entities.Where(c => c.GetComponent<SpawnerComponent>() != null).ToList();

            if (SpawnerEntities == null)
                throw new Exception("You must have spawner entities on arena levels.");

            //Cause I suck at figuring out the proper way to do this... There's gotta be a one line thing...
            foreach (Entity spawner in SpawnerEntities)
            {
                SpawnerComponents.Add(spawner.GetComponent<SpawnerComponent>());
            }
        }
    }
}
