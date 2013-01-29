using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Scenes {
	
	/// <summary>
	/// Provides a single system being used for a specific GameState.
	/// This class acts as a replacement to GameComponents, with CorvEngine features built in.
	/// </summary>
	public abstract class GameStateComponent : IGameComponent, IDrawable, IUpdateable {
		private GameState _GameState;
		private bool _Visible;
		private bool _Enabled;
		private int _DrawOrder;
		private int _UpdateOrder;

		public event EventHandler<EventArgs> DrawOrderChanged;
		public event EventHandler<EventArgs> VisibleChanged;
		public event EventHandler<EventArgs> EnabledChanged;
		public event EventHandler<EventArgs> UpdateOrderChanged;

		/// <summary>
		/// Creates a new GameStateComponent for the given State, but does not add it to the State's component list.
		/// </summary>
		public GameStateComponent(GameState State) {
			this._GameState = State;
		}

		/// <summary>
		/// Indicates the GameState that this Component is applied to.
		/// </summary>
		public GameState GameState {
			get { return _GameState; }
		}

		/// <summary>
		/// Indicates whether this component should continue to be updated while paused.
		/// The default value is false.
		/// </summary>
		public virtual bool UpdateWhilePaused {
			get { return false; }
		}

		/// <summary>
		/// Gets the instance of the engine that owns this game.
		/// </summary>
		public CorvEngine Engine {
			get { return CorvEngine.Instance; }
		}

		/// <summary>
		/// Indicates the order in which this component should be drawn, relative to the other components for this GameState.
		/// A higher order is drawn later.
		/// </summary>
		public int DrawOrder {
			get { return _DrawOrder; }
			set {
				if(_DrawOrder == value)
					return;
				_DrawOrder = value;
				if(this.DrawOrderChanged != null)
					this.DrawOrderChanged(this, new EventArgs());
			}
		}

		/// <summary>
		/// Indicates the order in which this component should be updated, relative to the other components for this GameState.
		/// A higher order is updated later.
		/// </summary>
		public int UpdateOrder {
			get { return _UpdateOrder; }
			set {
				if(_UpdateOrder == value)
					return;
				_UpdateOrder = value;
				if(this.UpdateOrderChanged != null)
					this.UpdateOrderChanged(this, new EventArgs());
			}
		}

		/// <summary>
		/// Indicates whether this component should be displayed, assuming the GameState is visible.
		/// </summary>
		public bool Visible {
			get { return _Visible; }
			set {
				if(value == _Visible)
					return;
				_Visible = value;
				if(this.VisibleChanged != null)
					this.VisibleChanged(this, new EventArgs());
			}
		}

		/// <summary>
		/// Indicates whether this component should be updated, assuming the GameState is not paused (unless UpdateWhenPaused is true).
		/// </summary>
		public bool Enabled {
			get { return _Enabled; }
			set {
				if(value == _Enabled)
					return;
				_Enabled = value;
				if(this.EnabledChanged != null)
					this.EnabledChanged(this, new EventArgs());
			}
		}

		/// <summary>
		/// Called when this component should be redrawn.
		/// </summary>
		protected abstract void OnDraw(GameTime Time);

		protected abstract void OnUpdate(GameTime Time);

		/// <summary>
		/// Explicitly defines the IGameComponent Initialize.
		/// This method should not be used by user code, and as such is sealed off.
		/// Instead, handle initialization in the constructor.
		/// </summary>
		void IGameComponent.Initialize() { 
			// We don't use this because GameComponent initialization is... odd, to say the least.
		}

		/// <summary>
		/// Dispatches a call to Draw to the implementation of GameStateComponent.
		/// Used for interacting with the Game class. This method should not be used by user code.
		/// </summary>
		void IDrawable.Draw(GameTime gameTime) {
			this.OnDraw(gameTime);
		}

		/// <summary>
		/// Dispatches a call to Update to the implementation of GameStateComponent.
		/// Used for interacting with the Game class. This method should not be used by user code.
		/// </summary>
		void IUpdateable.Update(GameTime gameTime) {
			this.OnUpdate(gameTime);
		}
	}
}
