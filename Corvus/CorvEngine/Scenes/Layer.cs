using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides information about a single layer within the game.
	/// </summary>
	public class Layer {

		/// <summary>
		/// Gets the tile with the given X and Y coordinates.
		/// These are the coordinates within the Tileset, so for example the second tile would be {1, 0}.
		/// If the coordinates are outside this layer, null is returned.
		/// Null may also be returned if this particular tile is empty for this layer.
		/// </summary>
		public Tile GetTile(int X, int Y) {
			if(X < 0 || Y < 0 || X >= _Tiles.GetLength(0) || Y >= _Tiles.GetLength(1))
				return null;
			return _Tiles[X, Y];
		}

		/// <summary>
		/// Gets the tile at the specified world coordinates.
		/// </summary>
		public Tile GetTileAtPosition(Vector2 Position) {
			var TileCoords = Position / _TileSize;
			return GetTile((int)TileCoords.X, (int)TileCoords.Y);
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
		/// Indicates if this layer is solid. In other words, if users should collide with tiles on this level.
		/// </summary>
		public bool IsSolid {
			get { return _Solid; }
			set { _Solid = value; }
		}

		/// <summary>
		/// Creates a new layer from the given tiles and size.
		/// </summary>
		public Layer(Vector2 TileSize, Tile[,] Tiles) {
			this._Tiles = Tiles;
			this._Scale = 1;
			this._TileSize = TileSize;
		}

		private Tile[,] _Tiles;
		private float _Scale;
		private bool _Solid = true;
		private Vector2 _TileSize;
	}
}
