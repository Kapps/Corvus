using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace System {
	/// <summary>
	/// Provides extensions used with CorvEngine.
	/// </summary>
	public static class CorvusExtensions {

		/// <summary>
		/// Loads the SpriteData located at the specified path, returning a fully created Sprite from the data.
		/// </summary>
		/// <param name="Content">The ContentManager to use.</param>
		/// <param name="AssetName">The path to the sprite, in the same format as for other Load calls.</param>
		public static Sprite LoadSprite(this ContentManager Content, string AssetName) {
			var SpriteData = Content.Load<SpriteData>(AssetName);
			return new Sprite(SpriteData);
		}

		/// <summary>
		/// Gets a scalar to multiply any movements by to make them based per-second.
		/// </summary>
		public static float GetTimeScalar(this GameTime Time) {
			return (float)Time.ElapsedGameTime.TotalSeconds;
		}
	}
}
