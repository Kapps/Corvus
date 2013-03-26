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
        int _Wave;
        int _TotalEnemiesSpawned;
        int _DifficultModifier;

        protected override void OnUpdate(GameTime Time)
        {

        }
    }
}
