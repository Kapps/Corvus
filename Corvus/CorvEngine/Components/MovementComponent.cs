using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities {
	public class MovementComponent : Component {
		float maxWalkVelocity = 500f;
		float maxJumpVelocity = 1050f;
		float gravity = 5000.5f;
		bool isJumping = false;
		bool isGrounded = false;
		bool jumpStart = false; //This flag is just essentially to account for the fact that we're grounded on the first jump. Could maybe do something like airtime too eventually.
		Direction CurrDir = Direction.Down;

		/// <summary>
		/// Walk in a certain direction. Handles animation. Needs to be called each update.
		/// </summary>
		/// <param name="dir"></param>
		public void Walk(Direction dir) {
			switch(dir) {
				case Direction.Left:
					this.Parent.VelX = maxWalkVelocity * -1;

					if(CurrDir != dir)
						Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + dir.ToString());

					break;
				case Direction.Right:
					this.Parent.VelX = maxWalkVelocity;

					if(CurrDir != dir)
						Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + dir.ToString());

					break;
				case Direction.Up:
					this.Parent.VelX = 0;
					//entity.VelY = 1000 * -1;
					break;
				case Direction.Down:
					this.Parent.VelX = 0;
					//entity.VelY = 1000;
					break;
				case Direction.None:
					this.Parent.VelX = 0;
					//entity.VelY = 0;

					if(CurrDir != dir)
						Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Idle" + CurrDir);

					break;
			}

			CurrDir = dir;
		}

		/// <summary>
		/// Start a jump. Sets necessary flags and adjusts Y velocity for a jump.
		/// </summary>
		/// <param name="allowMulti"></param>
		public void StartJump(bool allowMulti) {
			if((isJumping == false && isGrounded == true) || allowMulti) //Test if able to jump.
            {
				isJumping = true;
				isGrounded = false;
				jumpStart = true;
				Parent.VelY = maxJumpVelocity * -1 + 50;
			}
		}

		/// <summary>
		/// Just sets a flag to keep track of whether or not we're starting a jump in this update.
		/// </summary>
		private void EndStartJump() {
			jumpStart = false;
		}

		/// <summary>
		/// Handles walking and jumping, depending on flags and values.
		/// </summary>
		/// <param name="gameTime"></param>
		private void ApplyPhysics(GameTime gameTime) {
			//entity.X += entity.VelX * gameTime.GetTimeScalar();
			//entity.Y += entity.VelY * gameTime.GetTimeScalar();

			Vector2 PositionDelta = Parent.Velocity * gameTime.GetTimeScalar();

			if(!jumpStart) {
				bool AnyTileHit = false;
				bool AnySolidHit = false;
				foreach(var Layer in Parent.Scene.Layers) {
					Tile Tile = Layer.GetTileAtPosition(Parent.Position + new Vector2((Parent.Size / 2).X, Parent.Size.Y));
					if(Tile != null && Layer.IsSolid && Math.Abs(Parent.Location.Bottom - Tile.Location.Top) < Math.Abs(PositionDelta.Y + 0.1f)) {
						var TileAbove = Layer.GetTile((int)Tile.TileCoordinates.X, (int)Tile.TileCoordinates.Y - 1);
						if(TileAbove != null)
							continue; // Don't detect this as being hitting the floor because we're inside a spot that's solid wall.
						if(Parent.VelY < 0) // Don't 'fall' on to the tile if we're still going up.
							continue;
						Parent.Y = Tile.Location.Top - Tile.Location.Height;
						AnySolidHit = true;
						AnyTileHit = true;
					} else if(Tile != null)
						AnyTileHit = true;
				}

                if (AnySolidHit || !AnyTileHit)
                {
                    isGrounded = true;
                    jumpStart = false;
                    isJumping = false;
                    Parent.VelY = 0;
                    PositionDelta.Y = 0;
                }
                else
                {
                    isGrounded = false;
                }
			}

			Parent.Position += PositionDelta;

			if(!isGrounded) {
				Parent.VelY += gravity * gameTime.GetTimeScalar();
			}

			EndStartJump();
		}

		public override void Update(GameTime Time) {
			ApplyPhysics(Time);
			base.Update(Time);
		}
	}
}
