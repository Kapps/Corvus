using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;

namespace CorvEngine.Components {
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

		private float _Damage;

		protected override bool OnCollision(Entity Entity, EntityClassification Classification) {
			var HealthComponent = Entity.GetComponent<HealthComponent>();
			if(HealthComponent == null)
				return false;
			HealthComponent.CurrentHealth -= Damage;
			return true;
		}
	}
}
