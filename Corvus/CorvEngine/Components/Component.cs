using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides information about a single Component used in the Game.
	/// </summary>
	/// <remarks>
	/// All subclasses of Component should have a parameterless constructor for blueprints and serialization.
	/// </remarks>
	public class Component : SceneObject {

		// TODO: Component, System, and Entity need a new class.
		// At the very least it needs to have Update, Draw, Initialize, Dispose, and Scene.
		// Plus, events for things like Disposed and Initialized.

		/// <summary>
		/// Creates a new Component with no Parent yet.
		/// </summary>
		public Component() { }

		/// <summary>
		/// Gets the Entity that owns this Component.
		/// </summary>
		public Entity Parent {
			get { return _Parent; }
			internal set { _Parent = value; }
		}

		/// <summary>
		/// Indicates if only one instance of this Component can exist within an Entity.
		/// The default value is true.
		/// </summary>
		public virtual bool IsSingleInstance {
			get { return true; }
		}

		/// <summary>
		/// Returns the first Component of the specified type, throwing a MissingDependencyException if it's not found.
		/// In the future, this may be used to allow providing dependency graph information between Components, but that's not the case yet.
		/// </summary>
		protected T GetDependency<T>() where T : Component {
			// TODO: We should consider caching dependencies, since getting a component by a type is not exactly very efficient at the moment.
			var Result = Parent.Components[typeof(T)];
			if(Result == null)
				throw new MissingDependencyException(this, typeof(T));
			return (T)Result;
		}

		protected override void OnDispose() {
			if(Parent != null && !Parent.IsDisposed)
				Parent.Components.Remove(this.Name);
			base.OnDispose();
		}

		private Entity _Parent;
	}
}
