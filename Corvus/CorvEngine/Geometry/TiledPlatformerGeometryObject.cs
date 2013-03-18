using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Geometry {
	/// <summary>
	/// Provides a geometry object for a tiled platformer game.
	/// </summary>
	public class TiledPlatformerGeometryObject : ISceneGeometryObject {

		/// <summary>
		/// Gets or sets the location of this geometry object.
		/// </summary>
		public Rectangle Location { get; set; }


		public TiledPlatformerGeometryObject(Rectangle Location) {
			this.Location = Location;
		}
	}
}
