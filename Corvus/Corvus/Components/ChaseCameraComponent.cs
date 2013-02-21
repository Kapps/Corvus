using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;

namespace Corvus.Components {
	/// <summary>
	/// A Component used to center a given camera on the Entity owning this component.
	/// </summary>
	public class ChaseCameraComponent : Component {

		public ChaseCameraComponent(Camera Camera) {
			this._Camera = Camera;
		}

		/// <summary>
		/// Gets the camera that's being used to follow the Entity.
		/// </summary>
		public Camera Camera {
			get { return _Camera; }
		}

		protected override void OnUpdate(GameTime Time) {
			var EntityMiddle = new Vector2(Parent.Location.X + (Parent.Location.Width / 2), Parent.Location.Y + (Parent.Location.Height / 2));
			_Camera.Position = Parent.Position - (_Camera.Size / 2);
			base.OnUpdate(Time);
		}

		private Camera _Camera;
	}
}
