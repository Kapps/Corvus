using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides a reference to a node that contains an entity in order to allow for O(1) lookups, movements, and removals.
	/// </summary>
	internal class EntityNode {
		/// <summary>
		/// Gets the Entity that this node points to.
		/// </summary>
		public Entity Entity { get; private set; }

		/// <summary>
		/// Gets or sets the node reference.
		/// </summary>
		public object Node { get; set; }

		/// <summary>
		/// Creates an EntityNode for the given Entity.
		/// </summary>
		internal EntityNode(Entity Entity, object Node) {
			this.Entity = Entity;
			this.Node = Node;
		}
	}
}
