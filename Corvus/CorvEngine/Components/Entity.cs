using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CorvEngine.Components.Blueprints;
using CorvEngine.Graphics;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Components {

	/// <summary>
	/// Called when the location of an Entity changes (either the position or the size).
	/// </summary>
	public delegate void EntityLocationChangeDelegate(Entity Entity, Vector2 OldValue, Vector2 NewValue);

	/// <summary>
	/// Represents a single entity in the game.
	/// This class only provides a basic position within the world, leaving the remainder of game logic to be provided by Components.
	/// </summary>
	public class Entity : SceneObject {
		/// <summary>
		/// Gets an event called when the position of this entity changes.
		/// </summary>
		public event EntityLocationChangeDelegate PositionChanged;

		/// <summary>
		/// Gets an event called when the size of this entity changes.
		/// </summary>
		public event EntityLocationChangeDelegate SizeChanged;

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

        public float CentreX
        {
            get { return Position.X + (Size.X / 2); }
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
		/// Handles any updating of this Entity. The default implementation simply updates child Components.
		/// This is called automatically by the Scene.
		/// </summary>
		protected override void OnUpdate(GameTime Time) {
			foreach(var Component in this.Components)
				if(!Component.IsDisposed) // Possible to be disposed of by other updates.
					Component.Update(Time);
		}

		/// <summary>
		/// Draws this Entity. The default implementation simply renders all child Components.
		/// This is called automatically by the Scene.
		/// </summary>
		protected override void OnDraw() {
			foreach(var Component in this.Components)
				if(!Component.IsDisposed)
					Component.Draw();
		}

		protected override void OnInitialize() {
			base.OnInitialize();
			foreach(var Component in this.Components)
				Component.Initialize(Scene);
		}

		protected override void OnDispose() {
			base.OnDispose();
			foreach(var Component in this.Components)
				Component.Dispose();
		}

		public override string ToString() {
			return "Entity (" + this.Components.Count + " Component(s))";
		}

		private Vector2 _Position;
		private Vector2 _Size;
		private ComponentCollection _Components;
	}
}
