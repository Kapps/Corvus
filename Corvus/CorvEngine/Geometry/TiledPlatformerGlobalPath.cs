using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Geometry {
	/// <summary>
	/// Indicates how a global path should be followed for a 2D tile-based platformer game.
	/// </summary>
	public class TiledPlatformerGlobalPath : IGlobalPath {
		public TiledPlatformerGlobalPath(Queue<TiledPlatformerGeometryObject> Nodes) {
			this.Nodes = Nodes;
			this._CurrentNode = Nodes.Count > 0 ? Nodes.Dequeue() : null;
		}

		public ISceneGeometryObject CurrentNode {
			get { return _CurrentNode; }
		}

		public void AdvanceNode() {
			this._CurrentNode = Nodes.Dequeue();
		}

		private ISceneGeometryObject _CurrentNode;
		private Queue<TiledPlatformerGeometryObject> Nodes;
	}
}
