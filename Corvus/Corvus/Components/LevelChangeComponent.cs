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
    public class LevelChangeComponent : CollisionEventComponent
    {
        public string NextLevel
        {
            get { return _NextLevel; }
            set { _NextLevel = value; }
        }

        public string SpawnID
        {
            get { return _SpawnID; }
            set { _SpawnID = value; }
        }

        private string _NextLevel;
        private string _SpawnID = "";

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            if (string.IsNullOrEmpty(SpawnID))
                CorvusGame.Instance.SceneManager.ChangeScene(NextLevel, false);
            else
                CorvusGame.Instance.SceneManager.ChangeScene(NextLevel, false, SpawnID);
            
            return true;
        }
    }
}
