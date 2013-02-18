using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides information about a single layer within the game.
	/// </summary>
	public class Layer {

		/// <summary>
		/// Gets the tile with the given X and Y coordinates.
		/// These are the coordinates within the Tileset, so for example the second tile would be {1, 0}.
		/// </summary>
		public Tile GetTile(int X, int Y) {
			return _Tiles[X, Y];
		}

		/// <summary>
		/// Gets or sets the scale of this layer.
		/// Adjusting the scale of a layer affects all objects within it, and can give the illusion of 3D space when multiple layers with different scales are set.
		/// </summary>
		public float Scale {
			get { return _Scale; }
			set { _Scale = value; }
		}

		/// <summary>
		/// Creates a new layer from the given tiles.
		/// </summary>
		public Layer(Tile[,] Tiles) {
			this._Tiles = Tiles;
			this._Scale = 1;
		}

		private Tile[,] _Tiles;
		private float _Scale;
	}
}
