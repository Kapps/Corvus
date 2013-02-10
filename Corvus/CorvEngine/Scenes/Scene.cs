using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;
using Microsoft.Xna.Framework;
using NodeType = System.Collections.Generic.LinkedListNode<CorvEngine.Entities.Entity>;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides access to a scene that contains Entities for the game.
	/// </summary>
	public class Scene : GameStateComponent, IDisposable {
		// Eventually, this should be changed to use a QuadTree or Grid, but for now we don't need the performance.
		// Support does exist for plugging one in using an EntityNode reference though.
		private LinkedList<Entity> _Entities = new LinkedList<Entity>();

		public Scene(GameState State)
			: base(State) {

		}

		/// <summary>
		/// Adds the given Entity to this Scene.
		/// </summary>
		public void AddEntity(Entity Entity) {
			if(Entity.NodeReference != null)
				throw new ArgumentException("This Entity is already part of a different scene.");
			var Node = this._Entities.AddLast(Entity);
			Entity.NodeReference = new EntityNode(Entity, Node);
		}

		/// <summary>
		/// Removes the given Entity from this Scene.
		/// </summary>
		/// <param name="Entity"></param>
		public void RemoveEntity(Entity Entity) {
			var Node = (NodeType)Entity.NodeReference.Node;
			_Entities.Remove(Node);
			Entity.NodeReference = null;
		}

		/// <summary>
		/// Disposes of this Scene, removing all remaining Entities.
		/// </summary>
		public void Dispose() {
			_Entities.Clear();
		}

		protected override void OnDraw(GameTime Time) {
			foreach(var Entity in _Entities) {
				Entity.Draw(Time);
			}
		}

		protected override void OnUpdate(GameTime Time) {
			foreach(var Entity in _Entities) {
				Entity.Update(Time);
			}
		}
	}	
}
