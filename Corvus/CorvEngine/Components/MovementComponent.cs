using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities
{
    public class MovementComponent : Component
    {
        public float maxWalkVelocity = 500f;
        public float maxJumpVelocity = 1050f;
        public float gravity = 5000.5f;
        public bool isJumping = false;
        public bool isGrounded = false;
        public bool jumpStart = false; //This flag is just essentially to account for the fact that we're grounded on the first jump. Could maybe do something like airtime too eventually.
        public Direction CurrDir = Direction.Down;

        public void Walk(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    this.Parent.VelX = maxWalkVelocity * -1;

                    if (CurrDir != dir)
                        Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + dir.ToString());

                    break;
                case Direction.Right:
                    this.Parent.VelX = maxWalkVelocity;

                    if (CurrDir != dir)
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

                    if (CurrDir != dir)
                        Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Idle" + CurrDir);

                    break;
            }

            CurrDir = dir;
        }

        public void StartJump(bool allowMulti)
        {
            if ((isJumping == false && isGrounded == true) || allowMulti) //Test if able to jump.
            {
                isJumping = true;
                isGrounded = false;
                jumpStart = true;
                Parent.VelY = maxJumpVelocity * -1 + 50;
            }
        }

        public void EndStartJump()
        {
            jumpStart = false;
        }

		public void ApplyPhysics(GameTime gameTime, Scene Scene) {

			Vector2 PositionDelta = Parent.Velocity * gameTime.GetTimeScalar();
			Parent.Position += PositionDelta;
			//entity.X += entity.VelX * gameTime.GetTimeScalar();
			//entity.Y += entity.VelY * gameTime.GetTimeScalar();

			if(!jumpStart) {
				bool AnyTileHit = false;
				bool AnySolidHit = false;
				foreach(var Layer in Scene.Layers) {
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
				if(AnySolidHit || !AnyTileHit) {
					isGrounded = false;
					jumpStart = false;
					isJumping = false;
					Parent.VelY = 0;
				}
			}
			if(!isGrounded) {
				Parent.VelY += gravity * gameTime.GetTimeScalar();
			}
		}

        /*Old Physics
        if (entity.Y >= 1599.99 && mc.jumpStart != true) //Test if object is on ground and not beginning a jump.
        {
            mc.isGrounded = true;
            mc.isJumping = false;
            mc.jumpStart = false;
            entity.VelY = 0;
            entity.Y = 1600; //Just in case... Probably not needed.
        }*/
    }
}
