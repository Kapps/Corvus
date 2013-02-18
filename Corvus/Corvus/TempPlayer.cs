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

		public Entity entity;

		// TODO: Can make a component that does this.
		private Dictionary<Direction, Keys> DirToKey = new Dictionary<Direction, Keys>() {
			{ Direction.Left, Keys.Left },
			{ Direction.Right, Keys.Right },
			{ Direction.Up, Keys.Up },
			{ Direction.Down, Keys.Down },
		};

		public static TempPlayer TempInstance;

        MovementComponent mc = new MovementComponent();
		Scene Scene;
		public TempPlayer(Scene Scene) {
			// TODO: Complete member initialization
			// TODO: Move this away of course.
			TempInstance = this;
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
			entity.Size = new Vector2(40, 32);
			entity.Position = new Vector2(entity.Location.Width, Camera.Active.Viewport.Height);
            entity.Velocity = new Vector2(0, 0);
			entity.Initialize(null);
            mc = entity.GetComponent<MovementComponent>();
            mc.isGrounded = false;
		}

        Direction MoveDir = Direction.Down;

		public void Update(GameTime gameTime) {
			KeyboardState ks = Keyboard.GetState();

			// These should use binds of course.
			float Scalar = 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
			bool Any = false;
			foreach(var KVP in DirToKey) {
				if(ks.IsKeyDown(KVP.Value)) {
					if(MoveDir != KVP.Key) {
						MoveDir = KVP.Key;
					}
					Any = true;
					break;
				}
			}

			if(!Any && mc.CurrDir != Direction.None) {
                MoveDir = Direction.None;
			}

            mc.Walk(MoveDir); 

            if (ks.IsKeyDown(jump))
            {
                mc.StartJump(true);
            }

            mc.ApplyPhysics(gameTime, Scene); //Scene I would've thought was in entity already, but it's null. Bug or just me using it wrong?

            mc.EndStartJump();

			entity.Update(gameTime);



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