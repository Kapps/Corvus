using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TilePipeline {
	/// <summary>
	/// Provides content data for a Tileset.
	/// </summary>
	public class TileSetContent {
		[ContentSerializer]
		public Vector2 MapSize { get; set; }
		[ContentSerializer]
		public Vector2 TileSize { get; set; }
		[ContentSerializer]
		public Rectangle[,] SourceRects { get; set; }
	}
}
