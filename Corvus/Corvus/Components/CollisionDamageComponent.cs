using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace Corvus.Components {
	/// <summary>
	/// A Component used to deal damage when it collides with a different Entity.
	/// </summary>
	public class CollisionDamageComponent : CollisionEventComponent {
		/// <summary>
		/// Gets or sets the damage that this component should deal.
		/// </summary>
		public float Damage {
            get { return _Damage; }
            set { _Damage = value; }
		}

        /// <summary>
        /// Gets or sets a value that indicates whether to use this entity's attributes to apply damage.
        /// </summary>
        public bool UseAttributes
        {
            get { return _UseAttributes; }
            set { _UseAttributes = value; }
        }

        private float _Damage;
        private bool _UseAttributes = false;
        
		protected override bool OnCollision(Entity Entity, EntityClassification Classification) {
			var dc = Entity.GetComponent<DamageComponent>();
            if (dc == null)
				return false;
            if (!UseAttributes)
                dc.TakeDamage(this.Parent, Damage);
            else
            {
                var attri = this.GetDependency<AttributesComponent>();
                dc.TakeDamage(attri);
            }
			return true;
		}
	}
}
