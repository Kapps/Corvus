using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Geometry {
	/// <summary>
	/// Provides the base class for a global path that an Entity can follow.
	/// A global path is a path between objects, where as a local path is a path on that object.
	/// </summary>
	public interface IGlobalPath {
		/// <summary>
		/// Gets the current node node to be pathed to.
		/// </summary>
		ISceneGeometryObject CurrentNode { get;}

		/// <summary>
		/// Advances to the next node in the path.
		/// Local path finding is performed to reach the edge of the current node before moving on to the next node.
		/// </summary>
		void AdvanceNode();
	}
}
