using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides a system that's contained within a Scene.
	/// A system applies operations to Entities within the Scene, such as providing physics logic.
	/// While a Component can apply updates to itself, or render itself, it's recommended to use a System when dealing with multiple Entities.
	/// </summary>
	public abstract class System : IDisposable {

		// TODO: This Update/Draw method have been appearing in a great many places; they should maybe be an abstract class.

		/// <summary>
		/// Called when it's time to update this System.
		/// </summary>
		protected abstract void OnUpdate(GameTime Time);

		/// <summary>
		/// Called when it's time to render this System.
		/// </summary>
		protected abstract void OnDraw();

		/// <summary>
		/// Creates a new System owned by the specified Scene.
		/// </summary>
		public System(Scene Scene) {
			this._Scene = Scene;
			Scene.EntityAdded += Scene_EntityAdded;
			Scene.EntityRemoved += Scene_EntityRemoved;
		}

		/// <summary>
		/// Indicates if this System has been disposed.
		/// </summary>
		public bool IsDisposed {
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Gets the Scene that owns this System.
		/// </summary>
		public Scene Scene {
			get { return _Scene; }
		}

		/// <summary>
		/// Returns all components of the specified type, or derived from the specified type, within the Scene.
		/// Initially, this method will loop through each Component within the Scene, and add them to the filtered list.
		/// Afterwards, the list will be updated automatically as Components are added and removed, allowing for efficient lookups.
		/// </summary>
		protected IEnumerable<T> GetFilteredComponents<T>() where T : Component {
			ComponentFilter Filter = Filters.FirstOrDefault(c => c.ComponentType == typeof(T));
			if(Filter.ComponentType == null) { // Filter not found, default(ComponentFilter).
				// We're not tracking this type, so find every Component and return it.
				// If we find a Component, make sure that Entity is being tracked.
				List<Component> Components = new List<Component>();
				foreach(var Entity in Scene.Entities) {
					bool Any = false;
					foreach(var Component in Entity.Components) {
						var ComponentType = Component.GetType();
						if(ComponentType == typeof(T) || ComponentType.IsSubclassOf(typeof(T))) {
							Components.Add(Component);
							Any = true;
						}
					}
					if(Any)
						RegisterEntity(Entity);
				}
				Filter = new ComponentFilter(typeof(T), Components);
				Filters.Add(Filter);
			}
			// TODO: I'd imagine that this Select will pretty much destroy performance if we get too many Components.
			return Filter.Components.Select(c => (T)c);
		}

		/// <summary>
		/// Disposes of this System.
		/// </summary>
		public void Dispose() {
			if(_IsDisposed)
				return;
			_IsDisposed = true;
			Scene.RemoveSystem(this);
			OnDispose();
		}

		/// <summary>
		/// Called when this System is disposed.
		/// </summary>
		protected void OnDispose() { }

		internal void NotifyUpdate(GameTime Time) {
			OnUpdate(Time);
		}

		internal void NotifyDraw() {
			OnDraw();
		}

		void Scene_EntityRemoved(Entity obj) {
			UnregisterEntity(obj);
		}

		void Scene_EntityAdded(Entity obj) {
			RegisterEntity(obj);
		}

		private IEnumerable<ComponentFilter> GetFiltersForType(Type ComponentType) {
			// This can be multiple filters because of filter types deriving from Type.
			foreach(var Filter in Filters) {
				var Type = Filter.ComponentType;
				if(Type == ComponentType || ComponentType.IsSubclassOf(Type))
					yield return Filter;
			}
		}

		private void RegisterEntity(Entity Entity) {
			if(!EntitiesTracked.Add(Entity))
				return;
			foreach(var Component in Entity.Components) {
				var ComponentType = Component.GetType();
				foreach(var Filter in GetFiltersForType(ComponentType))
					Filter.Components.Add(Component);
			}
			Entity.Components.ComponentAdded += Components_ComponentAdded;
			Entity.Components.ComponentRemoved += Components_ComponentRemoved;
		}

		private void UnregisterEntity(Entity Entity) {
			if(!EntitiesTracked.Contains(Entity))
				return;
			foreach(var Component in Entity.Components) {
				var ComponentType = Component.GetType();
				foreach(var Filter in GetFiltersForType(ComponentType)) {
					bool Result = Filter.Components.Remove(Component);
					if(!Result)
						throw new KeyNotFoundException();
				}
			}
			Entity.Components.ComponentAdded -= Components_ComponentAdded;
			Entity.Components.ComponentRemoved -= Components_ComponentRemoved;
		}

		void Components_ComponentRemoved(Component obj) {
			foreach(var Filter in GetFiltersForType(obj.GetType())) {
				bool Result = Filter.Components.Remove(obj);
				if(!Result)
					throw new KeyNotFoundException();
			}
		}

		void Components_ComponentAdded(Component obj) {
			foreach(var Filter in GetFiltersForType(obj.GetType()))
				Filter.Components.Add(obj);
		}

		private List<ComponentFilter> Filters = new List<ComponentFilter>();
		private HashSet<Entity> EntitiesTracked = new HashSet<Entity>();
		private Scene _Scene;
		private bool _IsDisposed;


		struct ComponentFilter {
			public Type ComponentType;
			public List<Component> Components;

			public ComponentFilter(Type Type, List<Component> Components) {
				this.ComponentType = Type;
				this.Components = Components;
			}
		}
	}
}
