using CorvEngine.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corvus.Components
{
    public class ScoreComponent : Component
    {
        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        private int _Score = 0;
    }
}
