using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CorvEngine.Graphics;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Entities {

	/// <summary>
	/// Called when the location of an Entity changes (either the position or the size).
	/// </summary>
	public delegate void EntityLocationChangeDelegate(Entity Entity, Vector2 OldValue, Vector2 NewValue);

    /// <summary>
    /// Called when the velocity of an Entity changes .
    /// </summary>
    public delegate void EntityVelocityChangeDelegate(Entity Entity, Vector2 OldValue, Vector2 NewValue);

	/// <summary>
	/// Represents a single entity in the game.
	/// This class only provides a basic position within the world, leaving the remainder of game logic to be provided by Components.
	/// </summary>
	public class Entity : IDisposable {
		private Vector2 _Position;
		private Vector2 _Size;
        private Vector2 _Velocity;
		private ComponentCollection _Components;
		private bool _IsInitialized;
		private Scene _Scene;

		/// <summary>
		/// Gets an event called when the position of this entity changes.
		/// </summary>
		public event EntityLocationChangeDelegate PositionChanged;

		/// <summary>
		/// Gets an event called when the size of this entity changes.
		/// </summary>
		public event EntityLocationChangeDelegate SizeChanged;

        /// <summary>
        /// Gets an event called when the velocity of this entity changes.
        /// </summary>
        public event EntityVelocityChangeDelegate VelocityChanged;

		/// <summary>
		/// Provides a reference to the node that contains this Entity.
		/// </summary>
		internal EntityNode NodeReference { get; set; }

		/// <summary>
		/// Gets the Components that this Entity contains.
		/// </summary>
		public ComponentCollection Components {
			get { return _Components; }
		}

		/// <summary>
		/// Indicates if this Component has already been initialized.
		/// </summary>
		public bool IsInitialized {
			get { return _IsInitialized; }
		}

		/// <summary>
		/// Gets the Scene that this Entity is part of, if any.
		/// </summary>
		public Scene Scene {
			get { return _Scene; }
		}

		/// <summary>
		/// Gets or sets the size of this entity, in world space units.
		/// </summary>
		public Vector2 Size {
			get { return _Size; }
			set {
				if(_Size == value)
					return;
				Vector2 Old = _Size;
				_Size = value;
				if(SizeChanged != null)
					SizeChanged(this, Old, _Size);
			}
		}

		/// <summary>
		/// Gets or sets the position of this entity, in world space units.
		/// </summary>
		public Vector2 Position {
			get { return _Position; }
			set {
				if(_Position == value)
					return;
				Vector2 Old = _Position;
				_Position = value;
				if(PositionChanged != null)
					PositionChanged(this, Old, _Position);
			}
		}

		/// <summary>
		/// Gets the X coordinate of this Player.
		/// This translates calls to the Position property, and is simply short-hand for it.
		/// </summary>
		public float X {
			get { return _Position.X; }
			set { Position = new Vector2(value, Position.Y); }
		}

		/// <summary>
		/// Gets the Y coordinate of this Player.
		/// This translates calls to the Position property, and is simply short-hand for it.
		/// </summary>
		public float Y {
			get { return _Position.Y; }
			set { Position = new Vector2(Position.X, value); }
		}

		/// <summary>
		/// Gets the combined position and size of this entity.
		/// </summary>
		public Rectangle Location {
			get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y); }
		}

        /// <summary>
        /// Gets the velocity of this entity.
        /// </summary>
        public Vector2 Velocity
        {
            get { return _Velocity; }
            set {
                if (_Velocity == value)
                    return;
                Vector2 Old = _Velocity;
				_Velocity = value;
				if(VelocityChanged != null)
					PositionChanged(this, Old, _Velocity);
                }
        }

        /// <summary>
        /// Gets the velocity X of this entity.
        /// </summary>
        public float VelX
        {
            get { return _Velocity.X; }
            set { Velocity = new Vector2(value, Velocity.Y); }
        }

        /// <summary>
        /// Gets the velocity Y of this entity.
        /// </summary>
        public float VelY
        {
            get { return _Velocity.Y; }
            set { Velocity = new Vector2(Velocity.X, value); }
        }

		/// <summary>
		/// Creates a new Entity with no Components assigned.
		/// </summary>
		public Entity() {
			_Components = new ComponentCollection(this);
		}

		/// <summary>
		/// Returns the first component with the specified type.
		/// This is simply a shortcut for (T)Components[typeof(T)].
		/// </summary>
		/// <typeparam name="T">The type of the component.</typeparam>
		public T GetComponent<T>() where T : Component {
			var Component = this.Components[typeof(T)];
			return (T)Component;
		}

		/// <summary>
		/// Initializes this Entity. This is done by the Scene when this Entity is added to it.
		/// </summary>
		public void Initialize(Scene scene) {
			if(this.IsInitialized)
				throw new InvalidOperationException("Unable to initialize an Entity that has already been initialized.");
			this._IsInitialized = true;
			this._Scene = scene;
			foreach(var Component in this.Components)
				Component.Initialize();
			OnInitialize();
		}

		/// <summary>
		/// Handles any updating of this Entity.
		/// This is called automatically by the Scene.
		/// </summary>
		public virtual void Update(GameTime Time) {
			if(!IsInitialized)
				throw new InvalidOperationException("Unable to Update an Entity before it's initialized.");
			foreach(var Component in this.Components)
				Component.Update(Time);
		}

		/// <summary>
		/// Draws this Entity. The default implementation draws the Sprite with Color applied as a tint.
		/// This is called automatically by the Scene if the Entity is visible to the currently active Camera.
		/// </summary>
		public virtual void Draw() {
			foreach(var Component in this.Components)
				Component.Draw();
		}

		/// <summary>
		/// Called when this Entity is initialized for the first time, after all Components are initialized.
		/// </summary>
		protected virtual void OnInitialize() {

		}

		/// <summary>
		/// Disposes of this Entity and all of it's Components, removing them from the Scene.
		/// </summary>
		public void Dispose() {
			foreach(var Component in this.Components)
				Component.Dispose();
			// TODO: This will of course break things if it's Scene that's disposing us because we were removed.
			Scene.RemoveEntity(this);
		}

		public override string ToString() {
			return "Entity (" + this.Components.Count + " Component(s))";
		}
	}
}
