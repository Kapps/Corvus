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
    class CollisionCoinComponent : CollisionEventComponent
    {
        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var sc = Entity.GetComponent<ScoreComponent>();
            sc.Score++;
            return true;
        }
    }
}
