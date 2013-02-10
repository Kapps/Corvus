using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Entities {

	/// <summary>
	/// Called when the location of an Entity changes (either the position or the size).
	/// </summary>
	public delegate void EntityLocationChangeDelegate(Entity Entity, Vector2 OldValue, Vector2 NewValue);

	/// <summary>
	/// Represents a single entity in the game. This is the base class for most physical objects.
	/// </summary>
	public abstract class Entity {
		private Vector2 _Position;
		private Vector2 _Size;

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
		/// Gets the combined position and size of this entity.
		/// </summary>
		public Rectangle Location {
			get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y); }
		}

		/// <summary>
		/// Handles any updating of this Entity.
		/// This is called automatically by the Scene.
		/// </summary>
		public abstract void Update(GameTime Time);

		/// <summary>
		/// Draws this Entity.
		/// This is called automatically by the Scene.
		/// </summary>
		/// <param name="Time"></param>
		public abstract void Draw(GameTime Time);
	}
}
