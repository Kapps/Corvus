using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Entities {
	/// <summary>
	/// A component used to provide physics simulation for an Entity, including collisions.
	/// </summary>
	public class PhysicsComponent : Component {
		
		public override bool Visible {
			get {
				return false;
			}
		}

	}
}
