using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Input;
using Corvus.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Corvus.Components {
	/// <summary>
	/// A global component used to display debugging information, such as frame rate and position.
	/// </summary>
	public class DebugComponent : DrawableGameComponent {
		private SpriteFont Font;

		public DebugComponent() : base(CorvusGame.Instance.Game) {
			this.Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/TestFont");
			CorvusGame.Instance.PlayerAdded += Instance_PlayerAdded;
		}

		void Instance_PlayerAdded(Player Player) {
			Bind Bind = new Bind(Player.InputManager, ReloadLevel, false);
			Bind.RegisterButton(new InputButton(Keys.F5));
			Player.InputManager.RegisterBind(Bind);
		}

		public override void Draw(GameTime gameTime) {
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "FPS: " + CorvusGame.Instance.FPS, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow);
			if(CorvusGame.Instance.Players.Any())
				CorvusGame.Instance.SpriteBatch.DrawString(Font, "Position: " + CorvusGame.Instance.Players.First().Character.Position, new Vector2(10, GraphicsDevice.Viewport.Height - 50), Color.Yellow);
			base.Draw(gameTime);
		}

		private void ReloadLevel(BindState State) {
			if(State == BindState.Released)
				return;
			CorvusGame.Instance.SceneManager.ReloadScenes();
		}
	}
}
