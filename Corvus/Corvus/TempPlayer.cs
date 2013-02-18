using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorvEngine;
using CorvEngine.Entities;
using CorvEngine.Graphics;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Corvus {
	class TempPlayer : Player {
        // Note that this class is just a hackish mess used to test functionality until more is working.

        //Add these to entity or something else eventually?

        Keys jump = Keys.Space;

		Entity entity;

		// TODO: Can make a component that does this.
		private Dictionary<Direction, Keys> DirToKey = new Dictionary<Direction, Keys>() {
			{ Direction.Left, Keys.Left },
			{ Direction.Right, Keys.Right },
			{ Direction.Up, Keys.Up },
			{ Direction.Down, Keys.Down },
		};

        MovementComponent mc = new MovementComponent();
		Scene Scene;
		public TempPlayer(Scene Scene) {
			// TODO: Complete member initialization
			// TODO: Move this away of course.
			this.Scene = Scene;
			Camera.Active = this.Camera;
			Camera.Active.Size = new Vector2(CorvBase.Instance.GraphicsDevice.Viewport.Width, CorvBase.Instance.GraphicsDevice.Viewport.Height);
			SetupPlayer();
		}

		protected void SetupPlayer() {
			var Blueprint = EntityBlueprint.GetBlueprint("TestEntity");
			entity = Blueprint.CreateEntity();
			// This stuff is obviously things that the ctor should handle.
			// And things like size should probably be dependent upon the actual animation being played.
			entity.Size = new Vector2(48, 32);
			entity.Position = new Vector2(entity.Location.Width, Camera.Active.Viewport.Height);
            entity.Velocity = new Vector2(0, 0);
			entity.Initialize(null);
            mc = entity.GetComponent<MovementComponent>();
		}

		public void Update(GameTime gameTime) {
			KeyboardState ks = Keyboard.GetState();

			// These should use binds of course.
			float Scalar = 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
			bool Any = false;
			foreach(var KVP in DirToKey) {
				if(ks.IsKeyDown(KVP.Value)) {
					if(mc.CurrDir != KVP.Key) {
						entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + KVP.Value);
						mc.CurrDir = KVP.Key;
					}
					Any = true;
					break;
				}
			}

			if(!Any && mc.CurrDir != Direction.None) {
                entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Idle" + mc.CurrDir.ToString());
                mc.CurrDir = Direction.None;
			}

			switch(mc.CurrDir) {
				case Direction.Left:
                    entity.VelX = mc.maxWalkVelocity * -1;
					break;
				case Direction.Right:
                    entity.VelX = mc.maxWalkVelocity;
					break;
				case Direction.Up:
					entity.VelX = 0;
                    //entity.VelY = 1000 * -1;
					break;
				case Direction.Down:
					entity.VelX = 0;
					//entity.VelY = 1000;
					break;
                case Direction.None:
                    entity.VelX = 0;
                    //entity.VelY = 0;
                    break;
			}

            if (ks.IsKeyDown(jump))
            {
              //  if (mc.isJumping == false && mc.isGrounded == true) //Test if able to jump.
                //{
                    mc.isJumping = true;
                    mc.isGrounded = false;
                    mc.jumpStart = true;
                    entity.VelY = mc.maxJumpVelocity * -1 + 50;
                //}
            }

			if(!mc.isGrounded) {
				entity.VelY += mc.gravity * gameTime.GetTimeScalar();
			}
			entity.X += entity.VelX * gameTime.GetTimeScalar();
			entity.Y += entity.VelY * gameTime.GetTimeScalar();

			if(!mc.jumpStart) {
				bool AnyTileHit = false;
				bool AnySolidHit = false;
				foreach(var Layer in Scene.Layers) {
					Tile Tile = Layer.GetTileAtPosition(entity.Position + new Vector2((entity.Size / 2).X, entity.Size.Y));
					if(Tile != null && Layer.IsSolid && Math.Abs(entity.Location.Bottom - Tile.Location.Top) < 0.01f) {
						var TileAbove = Layer.GetTile((int)Tile.TileCoordinates.X, (int)Tile.TileCoordinates.Y - 1);
						if(TileAbove != null)
							continue; // Don't detect this as being hitting the floor because we're inside a spot that's solid wall.
						if(entity.VelY < 0) // Don't 'fall' on to the tile if we're still going up.
							continue;
						entity.Y = Tile.Location.Top - Tile.Location.Height; //Just in case... Probably not needed.
						AnySolidHit = true;
						AnyTileHit = true;
					} else if(Tile != null)
						AnyTileHit = true;
				}
				if(AnySolidHit || !AnyTileHit) {
					mc.isGrounded = false;
					mc.isJumping = false;
					mc.jumpStart = false;
					entity.VelY = 0;
				}
			}
			mc.jumpStart = false;

			entity.Update(gameTime);

            /*if (entity.Y >= 1599.99 && mc.jumpStart != true) //Test if object is on ground and not beginning a jump.
            {
                mc.isGrounded = true;
                mc.isJumping = false;
                mc.jumpStart = false;
                entity.VelY = 0;
                entity.Y = 1600; //Just in case... Probably not needed.
            }*/

			// Obviously this should just follow the player for the most part.
			// But for now, that would just make it seem like the player is never moving.
			var EntityMiddle = new Vector2(entity.Location.X + (entity.Location.Width / 2), entity.Location.Y + (entity.Location.Height / 2));
			Camera.Active.Position = EntityMiddle - (Camera.Active.Size / 2);
			/*if(entity.Location.Left < Camera.Active.Position.X)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X - 100, Camera.Active.Position.Y);
			if(entity.Location.Top < Camera.Active.Position.Y)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X, Camera.Active.Position.Y - 100);
			if(entity.Location.Bottom > Camera.Active.Position.Y + Camera.Active.Size.Y)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X, Camera.Active.Position.Y + 100);
			if(entity.Location.Right > Camera.Active.Position.X + Camera.Active.Size.X)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X + 100, Camera.Active.Position.Y);*/
		}

		public void Draw() {
			entity.Draw();
		}
	}
}