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
        private float _Knockback;
        private bool _UseAttributes = false;
        private DateTime _LastKnockback;
        /// <summary>
        /// Distance to knockback the entity.
        /// </summary>
        public float Knockback
        {
            get { return _Knockback; }
            set { _Knockback = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates to use this entities attributes.
        /// </summary>
        public bool UseAttributes
        {
            get { return _UseAttributes; }
            set { _UseAttributes = value; }
        }

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var mc = Entity.GetComponent<MovementComponent>();
            if (mc == null)
                return false;

            if ((DateTime.Now - _LastKnockback).TotalMilliseconds > 750)
            {
                _LastKnockback = DateTime.Now;
                float myx = Entity.Position.X - Parent.Position.X;
                float kb = (!UseAttributes) ? Knockback : this.GetDependency<AttributesComponent>().TotalKnockback;
                mc.Knockback(kb, (myx > 0) ? 1 : -1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
