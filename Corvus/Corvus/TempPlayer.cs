﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorvEngine;
using CorvEngine.Entities;
using CorvEngine.Graphics;
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
        float maxWalkVelocity = 5f;
        float maxJumpVelocity = 10f;
        float gravity = 0.5f;
        bool isJumping = false;
        bool isGrounded = true;
        bool jumpStart = false; //This flag is just essentially to account for the fact that we're grounded on the first jump. Could maybe do something like airtime too eventually.

        Keys jump = Keys.Space;

		Entity entity;
		enum Direction {
			None,
			Down,
			Left,
			Right,
			Up
		}
		// TODO: Can make a component that does this.
		private Dictionary<Direction, Keys> DirToKey = new Dictionary<Direction, Keys>() {
			{ Direction.Left, Keys.Left },
			{ Direction.Right, Keys.Right },
			{ Direction.Up, Keys.Up },
			{ Direction.Down, Keys.Down },
		};
		Direction CurrDir = Direction.Down;
		public TempPlayer() {
			// TODO: Complete member initialization
			// TODO: Move this away of course.
			Camera.Active = this.Camera;
			Camera.Active.Size = new Vector2(CorvBase.Instance.GraphicsDevice.Viewport.Width, CorvBase.Instance.GraphicsDevice.Viewport.Height);
			SetupPlayer();
		}

		protected void SetupPlayer() {
			BlueprintParser.ParseBlueprint(File.ReadAllText("TestEntity.txt"));
			var Blueprint = EntityBlueprint.GetBlueprint("TestEntity");
			entity = Blueprint.CreateEntity();
			// This stuff is obviously things that the ctor should handle.
			// And things like size should probably be dependent upon the actual animation being played.
			entity.Size = new Vector2(48, 32);
			entity.Position = new Vector2(entity.Location.Width, Camera.Active.Viewport.Height);
            entity.Velocity = new Vector2(0, 0);
			entity.Initialize(null);
		}

		public void Update(GameTime gameTime) {
			KeyboardState ks = Keyboard.GetState();

			// These should use binds of course.
			float Scalar = 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
			bool Any = false;
			foreach(var KVP in DirToKey) {
				if(ks.IsKeyDown(KVP.Value)) {
					if(CurrDir != KVP.Key) {
						entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + KVP.Value);
						CurrDir = KVP.Key;
					}
					Any = true;
					break;
				}
			}

			if(!Any && CurrDir != Direction.None) {
				entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Idle" + CurrDir.ToString());
				CurrDir = Direction.None;
			}

			switch(CurrDir) {
				case Direction.Left:
                    entity.VelX = maxWalkVelocity * -1;
					break;
				case Direction.Right:
                    entity.VelX = maxWalkVelocity;
					break;
				case Direction.Up:
                    //entity.VelY = maxJumpVelocity * -1;
					break;
				case Direction.Down:
                    //entity.VelY = maxJumpVelocity;
					break;
                case Direction.None:
                    entity.VelX = 0;
                    //entity.VelY = 0;
                    break;
			}

            if (ks.IsKeyDown(jump))
            {
                if (isJumping == false && isGrounded == true) //Test if able to jump.
                {
                    isJumping = true;
                    isGrounded = false;
                    jumpStart = true;
                    entity.VelY = maxJumpVelocity * -1;
                }
            }

            if (entity.Y >= (768 - 1) && jumpStart != true) //Test if object is on ground and not beginning a jump.
            {
                isGrounded = true;
                isJumping = false;
                jumpStart = false;
                entity.VelY = 0;
                entity.Y = 768; //Just in case... Probably not needed.
            }

            if (!isGrounded)
            {
                entity.VelY += gravity;
            }

            jumpStart = false;
            
            entity.X += entity.VelX;
            entity.Y += entity.VelY;
			
			entity.Update(gameTime);

			// Obviously this should just follow the player for the most part.
			// But for now, that would just make it seem like the player is never moving.
			if(entity.Location.Left < Camera.Active.Position.X)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X - 100, Camera.Active.Position.Y);
			if(entity.Location.Top < Camera.Active.Position.Y)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X, Camera.Active.Position.Y - 100);
			if(entity.Location.Bottom > Camera.Active.Position.Y + Camera.Active.Size.Y)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X, Camera.Active.Position.Y + 100);
			if(entity.Location.Right > Camera.Active.Position.X + Camera.Active.Size.X)
				Camera.Active.Position = new Vector2(Camera.Active.Position.X + 100, Camera.Active.Position.Y);
		}

		public void Draw() {
			entity.Draw();
		}
	}
}