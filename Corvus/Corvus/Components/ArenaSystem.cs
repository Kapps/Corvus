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

        public int TotalEntitiesKilled
        {
            get { return _TotalEntitiesKilled; }
            set { _TotalEntitiesKilled = value; }
        }

        public int TotalEntitiesKilledWave
        {
            get { return _TotalEntitiesKilledWave; }
            set { _TotalEntitiesKilledWave = value; }
        }

        private int _Wave = 1;
        private int _TotalEntitiesKilled = 0;
        private int _TotalEntitiesKilledWave = 0;

        private List<Entity> SpawnerEntities;
        private List<SpawnerComponent> SpawnerComponents = new List<SpawnerComponent>();

        protected override void OnUpdate(GameTime Time)
        {
            bool allEntitiesDisposed = true;
            bool allSpawnersComplete = true;

            //Go over each of the spawner components. 
            //Check if all entities are disposed and all spawners are finished spawning.
            //If just one isn't done or all entities were not disposed, we cannot complete the wave.
            foreach (SpawnerComponent spawnerComponent in SpawnerComponents)
            {
                if (spawnerComponent.TotalEntitiesToSpawn != spawnerComponent.TotalEntitiesSpawned)
                    allSpawnersComplete = false;

                foreach (Entity entity in spawnerComponent.EntitiesSpawned)
                {
                    if (entity.IsDisposed == false)
                    {
                        allEntitiesDisposed = false;
                    }
                }
            }

            if (allEntitiesDisposed && allSpawnersComplete)
            {
                foreach (SpawnerComponent spawnerComponent in SpawnerComponents)
                {
                    spawnerComponent.Reset();
                }

                TotalEntitiesKilledWave = 0;
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
