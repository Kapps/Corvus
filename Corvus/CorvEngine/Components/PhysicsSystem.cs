using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;

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

		/// <summary>
		/// Indicates the force of gravity.
		/// This number is currently fairly arbitrary, and should be tweaked as appropriate.
		/// </summary>
		public float Gravity {
			get { return _Gravity; }
			set { _Gravity = value; }
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time) {
			foreach(var Component in GetFilteredComponents<PhysicsComponent>()) {
				Vector2 PositionDelta = Component.Velocity * Time.GetTimeScalar();
				bool AnySolidHit = false;
				var Parent = Component.Parent;
				if(!Component.IsGrounded)
					Component.VelY += Gravity * Time.GetTimeScalar();
				foreach(var Layer in Scene.Layers.Where(c=>c.IsSolid)) {
					Tile Tile = Layer.GetTileAtPosition(Parent.Position + new Vector2((Parent.Size / 2).X, Parent.Size.Y));
					if(Tile != null && Layer.IsSolid && Math.Abs(Parent.Location.Bottom - Tile.Location.Top) < Math.Abs(PositionDelta.Y) + 1.1f) {
						var TileAbove = Layer.GetTile((int)Tile.TileCoordinates.X, (int)Tile.TileCoordinates.Y - 1);
						if(TileAbove != null)
							continue; // Don't detect this as being hitting the floor because we're inside a spot that's solid wall.
						if(Component.VelY < 0) // Don't 'fall' on to the tile if we're still going up.
							continue;
						Parent.Y = Tile.Location.Top - Tile.Location.Height;
						AnySolidHit = true;
					}
				}

				if(AnySolidHit) { //If hitting any solid object OR not hitting any tile, ground us.
					Component.IsGrounded = true;
					Component.VelY = 0;
					PositionDelta.Y = 0;
				} else { //We're not yet on ground.
					Component.IsGrounded = false;
				}

				Parent.Position += PositionDelta;
				Parent.Y = Math.Max(0, Parent.Y);
				Parent.X = Math.Max(-Scene.TileSize.X / 2, Parent.X);
				Parent.Y = Math.Min(Scene.MapSize.Y - Scene.TileSize.Y, Parent.Y);
				Parent.X = Math.Min(Scene.MapSize.X - Scene.TileSize.X, Parent.X);
			}
		}

		protected override void OnDraw() {
			
		}

		private float _Gravity = 5000;
	}
}
