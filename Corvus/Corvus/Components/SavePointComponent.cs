using CorvEngine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corvus.Components {
	/// <summary>
	/// Provides a component that triggers a level save when the user walks over it.
	/// </summary>
	public class SavePointComponent : CollisionEventComponent {


		protected override bool OnCollision(Entity Entity, EntityClassification Classification) {
			throw new NotImplementedException();
		}
	}
}
