using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;

namespace CorvEngine.Components
{
    public class ArenaSystem : SceneSystem
    {
        public int Wave
        {
            get { return _Wave; }
        }

        private int _Wave;
        private int _TotalEnemiesWave;

        //Unsure about these being here, but fine for now.
        private int _TotalEnemiesSpawned;
        private int _TotalEnemiesKilled;
        private int _TotalEnemiesSpawnedWave;
        private int _TotalEnemiesKilledWave;

        private int _DifficultyModifier; //This'll actually just be calculated by wave most likely, so probably unnecessary.

        protected override void OnUpdate(GameTime Time)
        {

        }
    }
}
