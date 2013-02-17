using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides information about a single Component used in the Game.
	/// </summary>
	/// <remarks>
	/// All subclasses of Component should have a parameterless constructor for blueprints and serialization.
	/// </remarks>
	public class Component : IDisposable {

		/// <summary>
		/// Gets an event called when this Component is disposed of.
		/// </summary>
		public event Action<Component> Disposed;

		/// <summary>
		/// Creates a new Component with no Parent yet assigned and the specified name.
		/// If Name is null, a name is generated.
		/// </summary>
		public Component(string Name = null) {
			int CurrID = Interlocked.Increment(ref NextComponentID);
			this._ID = CurrID;
			this._Name = Name ?? (this.GetType().Name + CurrID);
		}
		
		/// <summary>
		/// Returns an identifier for this Component that is guaranteed to never change and be unique amongst all Components, not just those within this Entity.
		/// </summary>
		public int ID {
			get { return _ID; }
		}

		/// <summary>
		/// Gets the Entity that owns this Component.
		/// </summary>
		public Entity Parent {
			get { return _Parent; }
			internal set { _Parent = value; }
		}

		/// <summary>
		/// Indicates if this Component has been disposed of.
		/// </summary>
		public bool IsDisposed {
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Indicates if only one instance of this Component can exist within an Entity.
		/// The default value is true.
		/// </summary>
		public virtual bool IsSingleInstance {
			get { return true; }
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
		/// Disposes of this Component, removing it from the Entity's Components list if applicable.
		/// </summary>
		public void Dispose() {
			if(_IsDisposed)
				throw new ObjectDisposedException("Unable to dispose of a disposed object.", (Exception)null);
			this._IsDisposed = true;
			if(Parent != null)
				Parent.Components.Remove(this.Name);
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
		/// Initializes this Component.
		/// This is called after the Entity itself is initialized by becoming part of the Scene.
		/// If this Component was added after the Entity was already part of the Scene, this method is called immediately.
		/// </summary>
		public virtual void Initialize() {
			if(_IsInitialized)
				throw new InvalidOperationException("Unable to initialize a Component that has already been initialized.");
			_IsInitialized = true;
		}

		/// <summary>
		/// Advances this Component by the specified period of time.
		/// </summary>
		public virtual void Update(GameTime Time) { }

		/// <summary>
		/// Renders this Component.
		/// Note that this may be called multiple times per frame.
		/// </summary>
		public virtual void Draw() { }

		/// <summary>
		/// Returns the first Component of the specified type, throwing a MissingDependencyException if it's not found.
		/// In the future, this may be used to allow providing dependency graph information between Components, but that's not the case yet.
		/// </summary>
		protected Component GetDependency(Type ComponentType) {
			var Result = Parent.Components[ComponentType];
			if(Result == null)
				throw new MissingDependencyException(this, ComponentType);
			return Result;
		}

		public override string ToString() {
			return this.Name + " (" + this.GetType().Name + ")";
		}

		private Entity _Parent;
		private bool _IsInitialized;
		private bool _IsDisposed;
		private string _Name;
		private int _ID;
		private static int NextComponentID = 0;
	}
}
