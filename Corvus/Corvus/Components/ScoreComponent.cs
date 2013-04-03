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
        public int Coins
        {
            get { return _Coins; }
            set { _Coins = value; }
        }

        private int _Coins = 0;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}
