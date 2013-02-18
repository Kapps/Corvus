using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus {
	/// <summary>
	/// A global component used to display the current FPS.
	/// </summary>
	public class FrameRateComponent : DrawableGameComponent {
		private SpriteFont Font;

		public FrameRateComponent() : base(CorvusGame.Instance.Game) {
			this.Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/TestFont");
		}

		public override void Draw(GameTime gameTime) {
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "FPS: " + CorvusGame.Instance.FPS, new Vector2(10, GraphicsDevice.Viewport.Height - 20), Color.Yellow);
			base.Draw(gameTime);
		}
	}
}
