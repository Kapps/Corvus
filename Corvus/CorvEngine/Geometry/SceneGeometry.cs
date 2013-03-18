using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Scenes;

namespace CorvEngine.Geometry {
	/// <summary>
	/// Provides the base class for a LevelGeometry that can be used to calculate a path for path-finding.
	/// Implementations are provided for tiled platformers (TiledPlatformerGeometry).
	/// Other implementations will require extending LevelGeometry to determine MovementActions as appropriate.
	/// </summary>
	public abstract class SceneGeometry {

		// At the moment, there is no support for Entities that are solid becoming not solid, and the other way around, short of a dispose.
		// TODO: Perhaps we should make this thread safe and make it operate separately from updates.
		// After all, we may not need to update paths every update call.

		/// <summary>
		/// Gets the scene that's being used for this geometry.
		/// </summary>
		public Scene Scene { get; private set; }

		/// <summary>
		/// Creates a new SceneGeometry for the specified Scene.
		/// </summary>
		public SceneGeometry(Scene Scene) {
			this.Scene = Scene;
			Scene.EntityAdded += Scene_EntityAdded;
		}

		void Scene_EntityAdded(Entity obj) {
			if(obj.IsInitialized)
				OnEntityInitialized(obj);
			else
				obj.Initialized += (scobj) => OnEntityInitialized((Entity)scobj);
		}

		/// <summary>
		/// Called when an Entity that's part of the Scene is initialized.
		/// The default implementation creates a SceneGeometryObject to add to the SceneGeometry.
		/// </summary>
		protected void OnEntityInitialized(Entity Entity) {
			var Geometry = CreateGeometryForEntity(Entity);
			if(Geometry != null) {
				_EntityGeometries.Add(Entity, Geometry);
				AddGeometry(Geometry);
				Entity.Disposed += (scobj) => OnEntityDisposed((Entity)scobj);
			}
		}

		/// <summary>
		/// Called when an initialized Entity is disposed.
		/// The default implementation removes the Entity's geometry from the SceneGeometry.
		/// </summary>
		/// <param name="Entity"></param>
		protected void OnEntityDisposed(Entity Entity) {
			var Geometry = GetGeometryForEntity(Entity);
			if(Geometry == null)
				return;
			_EntityGeometries.Remove(Entity);
			RemoveGeometry(Geometry);
		}

		/// <summary>
		/// Returns the GeometryObject that was created for the specified Entity, or null if the Entity had no existing geometry.
		/// This is handled internally by the SceneGeometry when an Entity is initialized and CreateGeometryForEntity does not return null.
		/// </summary>
		protected virtual ISceneGeometryObject GetGeometryForEntity(Entity Entity) {
			ISceneGeometryObject Result;
			if(!_EntityGeometries.TryGetValue(Entity, out Result))
				return null;
			return Result;
		}

		/// <summary>
		/// Override to handle creating the geometry for an Entity.
		/// This should return null if no geometry should be created for this Entity.
		/// </summary>
		protected abstract ISceneGeometryObject CreateGeometryForEntity(Entity Entity);

		/// <summary>
		/// Called when the specified geometry should be removed from this Scene's geometry.
		/// </summary>
		protected abstract void RemoveGeometry(ISceneGeometryObject Object);

		/// <summary>
		/// Called when the specified geometry should be added to this Scene's geometry.
		/// </summary>
		protected abstract void AddGeometry(ISceneGeometryObject Object);

		private Dictionary<Entity, ISceneGeometryObject> _EntityGeometries = new Dictionary<Entity, ISceneGeometryObject>();
	}
}
