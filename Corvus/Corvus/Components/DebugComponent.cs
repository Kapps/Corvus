using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components {
	/// <summary>
	/// A global component used to display debugging information, such as frame rate and position.
	/// </summary>
	public class DebugComponent : DrawableGameComponent {
		private SpriteFont Font;

		public DebugComponent() : base(CorvusGame.Instance.Game) {
			this.Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/TestFont");
		}

		public override void Draw(GameTime gameTime) {
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "FPS: " + CorvusGame.Instance.FPS, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow);
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "Position: " + CorvusGame.Instance.Players.First().Character.Position, new Vector2(10, GraphicsDevice.Viewport.Height - 50), Color.Yellow);
			base.Draw(gameTime);
		}
	}
}
