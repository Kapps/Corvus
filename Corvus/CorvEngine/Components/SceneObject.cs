using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides the base class for an object that is part of a Scene, such as a System, Component, or Entity.
	/// </summary>
	public abstract class SceneObject {

		// TODO: Still more we can move here.
		// For example, a parent and children.
		// Scene in particular can benefit from this.

		/// <summary>
		/// Gets an event called every time this object is disposed of.
		/// </summary>
		public event Action<SceneObject> Disposed;

		/// <summary>
		/// Gets an event called every time this object is initialized.
		/// </summary>
		public event Action<SceneObject> Initialized;

		/// <summary>
		/// Indicates if this Component has been disposed of.
		/// </summary>
		public bool IsDisposed {
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Indicates if this Component should be drawn. The default value is true.
		/// </summary>
		public virtual bool Visible {
			get { return true; }
		}

		/// <summary>
		/// Indicates if this Component should be updated. The default value is true.
		/// </summary>
		public virtual bool Enabled {
			get { return true; }
		}

		/// <summary>
		/// Indicates if this Component has already been initialized.
		/// </summary>
		public bool IsInitialized {
			get { return _IsInitialized; }
		}

		/// <summary>
		/// Returns an identifier for this SceneObject that is guaranteed to never change and be unique amongst all SceneObjects, not just those within this Scene.
		/// </summary>
		public int ID {
			get { return _ID; }
		}

		/// <summary>
		/// Gets the Scene that this SceneObject is part of, or null if the SceneObject is not currently attached to a Scene.
		/// </summary>
		public Scene Scene {
			get { return _Scene; }
		}

		/// <summary>
		/// Creates a new SceneObject with a unique name.
		/// </summary>
		public SceneObject() {
			int CurrID = Interlocked.Increment(ref NextObjectID);
			this._ID = CurrID;
			this._Name = Name ?? (this.GetType().Name + CurrID);
		}

		/// <summary>
		/// Disposes of this object, removing it from the Scene if it has been initialized.
		/// It is possible for a SceneObject to be reinitialized after being Disposed if added to a different Scene.
		/// In this case, Initialize will be called again with the new Scene.
		/// </summary>
		public void Dispose() {
			if(_IsDisposed)
				throw new ObjectDisposedException("Unable to dispose of an already disposed object without a call to Initialize between the Disposes..", (Exception)null);
			this._IsDisposed = true;
			this._IsInitialized = false;
			this._Scene = null;
			OnDispose();
			if(this.Disposed != null)
				this.Disposed(this);
		}

		/// <summary>
		/// Gets the name of this Components. Components are accessible by either type or name.
		/// </summary>
		public virtual string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Initializes this SceneObject to the specified Scene.
		/// This is called when the SceneObject is added to the Scene, or if the SceneObject's parent is already initialized, is called immediately.
		/// </summary>
		public void Initialize(Scene Scene) {
			if(_IsInitialized)
				throw new InvalidOperationException("Unable to initialize a SceneObject that has already been initialized.");
			this._IsInitialized = true;
			this._IsDisposed = false;
			this._Scene = Scene;
			OnInitialize();
			if(this.Initialized != null)
				this.Initialized(this);
		}

		/// <summary>
		/// Advances this object by the specified period of time.
		/// This is called only once per frame.
		/// </summary>
		public void Update(GameTime Time) {
			if(_IsDisposed || !_IsInitialized)
				throw new InvalidOperationException("Unable to update an object that is either disposed or not initialized.");
			OnUpdate(Time);
		}

		/// <summary>
		/// Renders this object.
		/// Note that this may be called multiple times per frame.
		/// </summary>
		public void Draw() {
			if(_IsDisposed || !_IsInitialized)
				throw new InvalidOperationException("Unable to update an object that is either disposed or not initialized.");
			OnDraw();
		}

		/// <summary>
		/// Called when this object is initialized, either for the first time or after a call to Dispose.
		/// </summary>
		protected virtual void OnInitialize() {

		}

		/// <summary>
		/// Called when this object is disposed.
		/// An object may be disposed of without currently being initialized, and may be disposed of again each time it's reinitialized.
		/// </summary>
		protected virtual void OnDispose() {

		}

		/// <summary>
		/// Called when this object needs to be updated by the specified amount of time.
		/// </summary>
		protected virtual void OnUpdate(GameTime Time) {

		}

		/// <summary>
		/// Called when this object needs to be rendered.
		/// </summary>
		protected virtual void OnDraw() {

		}

		public override string ToString() {
			return this.Name + " (" + this.GetType().Name + ")";
		}

		private bool _IsInitialized;
		private bool _IsDisposed;
		private string _Name;
		private int _ID;
		private Scene _Scene;
		private static int NextObjectID = 0;
	}
}
