using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorvEngine.Components;

namespace Corvus.Components
{
    //TODO: Maybe it is better to merge this with CollisionDamageComponent. (or possibly merge all the Collision Components)

    /// <summary>
    /// A Component used to knockback an entity when it collides with this entity.
    /// </summary>
    public class CollisionKnockbackComponent : CollisionEventComponent
    {
        /// <summary>
        /// Distance to knockback the entity.
        /// </summary>
        public float Knockback
        {
            get { return _Knockback; }
            set { _Knockback = value; }
        }

        private float _Knockback;

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            if (Knockback == 0)
                return false;
            var mc = Entity.GetComponent<MovementComponent>();
            if (mc == null)
                return false;
            float myx = Entity.Position.X - Parent.Position.X;
            mc.Knockback(Knockback, (myx> 0) ? 1: -1);
            return true;
        }
    }
}
