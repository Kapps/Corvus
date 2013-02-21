using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
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
			var CamPos = Parent.Position - (_Camera.Size / 2);
			CamPos.X = Math.Max(CamPos.X, 0);
			CamPos.Y = Math.Max(CamPos.Y, 0);
			CamPos.X = Math.Min(CamPos.X, Scene.MapSize.X - _Camera.Size.X);
			CamPos.Y = Math.Min(CamPos.Y, Scene.MapSize.Y - _Camera.Size.Y);
			_Camera.Position = CamPos;
			base.OnUpdate(Time);
		}

		private Camera _Camera;
	}
}
