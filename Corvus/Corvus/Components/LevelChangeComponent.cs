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
    class LevelChangeComponent : CollisionEventComponent
    {
        public string PrevLevel
        {
            get { return _PrevLevel; }
            set { _PrevLevel = value; }
        }

        public string NextLevel
        {
            get { return _NextLevel; }
            set { _NextLevel = value; }
        }

        private string _PrevLevel;
        private string _NextLevel;

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            if (Entity.GetComponent<ClassificationComponent>().Classification == EntityClassification.Player)
                CorvusGame.Instance.SceneManager.ChangeScene(NextLevel, false);

            return true;
        }
    }
}
