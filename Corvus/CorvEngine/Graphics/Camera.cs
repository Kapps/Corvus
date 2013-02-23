using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides an implementation of a Camera that allows for determining whether to draw an object.
	/// </summary>
	public class Camera {
		private static Camera _Active;
		private Vector2 _Position;
		private Vector2 _Size;
		
		/// <summary>
		/// Gets or sets the currently active camera.
		/// That is, the camera that will be used for any current rendering.
		/// </summary>
		public static Camera Active {
			get { return _Active; }
			set { _Active = value; }
		}

		/// <summary>
		/// Gets or sets the position of the Camera.
		/// This is the top-left most point that will be within the screen.
		/// </summary>
		public Vector2 Position {
			get { return _Position; }
			set { _Position = value; }
		}

		/// <summary>
		/// Gets or sets the size of the camera.
		/// </summary>
		public Vector2 Size {
			get { return _Size; }
			set { _Size = value; }
		}

		/// <summary>
		/// Creates a new Camera at {0, 0}, with {ViewportWidth, ViewportHeight} as the Position and Size.
		/// </summary>
		public Camera() {
			this.Position = new Vector2(0, 0);
			this.Size = new Vector2(CorvBase.Instance.GraphicsDevice.Viewport.Width, CorvBase.Instance.GraphicsDevice.Viewport.Height);
		}

		/// <summary>
		/// Gets a rectangle encompassing the Camera's viewport.
		/// </summary>
		public Rectangle Viewport {
			get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y); }
		}

		/// <summary>
		/// Indicates whether the Viewport of this Camera contains or intersects the location of the given Entity.
		/// </summary>
		public bool Contains(Entity Entity) {
			return this.Viewport.Intersects(Entity.Location);
		}

		/// <summary>
		/// Maps the given world coordinates to screen coordinates.
		/// </summary>
		public Vector2 ScreenToWorld(Vector2 Position) {
			return new Vector2(Position.X - this.Position.X, Position.Y - this.Position.Y);
		}
	}
}
