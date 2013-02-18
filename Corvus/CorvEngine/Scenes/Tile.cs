using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Represents a single tile within a layer.
	/// </summary>
	public class Tile {

		/// <summary>
		/// Gets the texture being used for this tile.
		/// </summary>
		public Texture2D Texture { get; private set; }

		/// <summary>
		/// Gets the source rectangle within the texture for this tile.
		/// </summary>
		public Rectangle SourceRect { get; private set; }

		/// <summary>
		/// Gets the location of this Tile in world-space.
		/// </summary>
		public Rectangle Location { get; private set; }

		public Tile(Texture2D Texture, Rectangle SourceRect, Rectangle Location) {
			this.Texture = Texture;
			this.SourceRect = SourceRect;
			this.Location = Location;
		}
	}
}
