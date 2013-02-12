using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
		private Sprite _Sprite;
		private Color _Color = Color.White;

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
		/// Gets or sets the color to apply to the sprite being used for this entity.
		/// </summary>
		public Color Color {
			get { return _Color; }
			set { _Color = value; }
		}

		/// <summary>
		/// Gets or sets the sprite being used for this entity.
		/// </summary>
		public Sprite Sprite {
			get { return _Sprite; }
			set { _Sprite = value; }
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
		public virtual void Update(GameTime Time) {
			
		}

		/// <summary>
		/// Draws this Entity. The default implementation draws the Sprite with Color applied as a tint.
		/// This is called automatically by the Scene if the Entity is visible to the currently active Camera.
		/// </summary>
		public virtual void Draw(GameTime Time) {
			var SpriteBatch = CorvBase.Instance.SpriteBatch;
			Vector2 ScreenPosition = Camera.Active.ScreenToWorld(this.Position);
			var ActiveFrame = Sprite.ActiveAnimation.ActiveFrame.Frame;
			var SourceRect = ActiveFrame.Source;
			SpriteBatch.Draw(this.Sprite.Texture, new Rectangle((int)ScreenPosition.X, (int)ScreenPosition.Y, (int)Size.X, (int)Size.Y), SourceRect, Color);
		}
	}
}
