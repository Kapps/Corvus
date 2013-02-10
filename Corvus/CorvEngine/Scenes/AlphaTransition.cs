using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides a GameStateTransition that uses alpha blending to transition between the two GameStates.
	/// </summary>
	public class AlphaTransition : GameStateTransition {
		private SpriteBatch SpriteBatch;

		/// <summary>
		/// Creates a new instance of AlphaTransition.
		/// </summary>
		public AlphaTransition() {
			this.SpriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override TimeSpan TotalTime {
			get { return TimeSpan.FromSeconds(1); }
		}

		protected override void RenderTransition(RenderTarget2D From, RenderTarget2D To, TransitionMode Mode, TimeSpan Elapsed) {
			float ToLerp = Math.Min(Elapsed.Ticks / TotalTime.Ticks, 1);
			float FromLerp = 1.0f - ToLerp;
			SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			if(From != null) 
				SpriteBatch.Draw(From, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(1f, 1f, 1f, 1f - FromLerp));
			if(To != null)
				SpriteBatch.Draw(To, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(1f, 1f, 1f, 1f - ToLerp));
			SpriteBatch.End();
		}
	}
}
