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
    public class CollisionCoinComponent : CollisionEventComponent
    {
        public int Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private int _Value;

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var sc = Entity.GetComponent<ScoreComponent>();
            sc.Coins += Value;
            var ftc = Entity.GetComponent<FloatingTextComponent>();
            if (ftc != null)
                ftc.Add("+" + Value.ToString(), Color.Gold);
            AudioManager.PlaySoundEffect("CoinPickup");
            Parent.Dispose();
            return true;
        }
    }
}
