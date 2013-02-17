using System;
using System.Collections.Generic;
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

		Entity entity;
		enum Direction {
			None,
			Down,
			Left,
			Right,
			Up
		}
		private Dictionary<Direction, Keys> DirToKey = new Dictionary<Direction, Keys>() {
			{ Direction.Left, Keys.Left },
			{ Direction.Right, Keys.Right },
			{ Direction.Up, Keys.Up },
			{ Direction.Down, Keys.Down }
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
			entity = new Entity();
			var Sprite = CorvusGame.Instance.GlobalContent.LoadSprite("Sprites/TestPlayer");
			entity.Components.Add(new SpriteComponent(Sprite));
			// This stuff is obviously things that the ctor should handle.
			// And things like size should probably be dependent upon the actual animation being played.
			entity.Size = new Vector2(48, 32);
			entity.Position = new Vector2(entity.Location.Width, Camera.Active.Viewport.Height);
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
				}
			}
			if(!Any && CurrDir != Direction.None) {
				entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Idle" + CurrDir.ToString());
				CurrDir = Direction.None;
			}
			switch(CurrDir) {
				case Direction.Left:
					entity.X -= Scalar;
					break;
				case Direction.Right:
					entity.X += Scalar;
					break;
				case Direction.Up:
					entity.Y -= Scalar;
					break;
				case Direction.Down:
					entity.Y += Scalar;
					break;
			}
			
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