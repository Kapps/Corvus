using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Scenes;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides a System used to manage physics for Entities that have a PhysicsComponent.
	/// </summary>
	public class PhysicsSystem : System {

		/// <summary>
		/// Creates a new PhysicsSystem for the given Scene.
		/// </summary>
		public PhysicsSystem(Scene Scene)
			: base(Scene) {

		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time) {
			
		}

		protected override void OnDraw() {
			
		}
		
	}
}
