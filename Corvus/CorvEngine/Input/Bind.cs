using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CorvEngine.Input {
	
	/// <summary>
	/// Indicates the state of a bind.
	/// </summary>
	public enum BindState {
		/// <summary>
		/// The bind is currently being pressed down.
		/// </summary>
		Pressed,
		/// <summary>
		/// The bind is not being pressed down.
		/// </summary>
		Released
	}

	/// <summary>
	/// Provides an EventArgs value for when a bind is invoked, optionally preventing the action from being invoked.
	/// </summary>
	public class BindPressEventArgs : EventArgs {
		/// <summary>
		/// Indicates whether this event has been handled, preventing the mapped action from being invoked.
		/// </summary>
		public bool Handled { get; set; }

		/// <summary>
		/// Gets the action that should be invoked for this press.
		/// </summary>
		public Action<BindState> Action { get; set; }

		/// <summary>
		/// Gets the player that triggered this bind.
		/// </summary>
		public Player Player { get; set; }
	}

	/// <summary>
	/// An object used to associate one or more keys/button with an action, invoking the action and raising events when the button is pressed.
	/// </summary>
	public class Bind {
		private List<InputButton> _Buttons = new List<InputButton>();
		private Action<BindState> _Action;
		private InputManager _InputManager;
		private BindState _State = BindState.Released;
		private bool _MultiInvoke = false;

		/// <summary>
		/// An event invoked when one or more of the InputButton values required for this Bind to be triggered are pressed.
		/// Called immediately before Action is invoked.
		/// </summary>
		public event EventHandler Pressed;

		/// <summary>
		/// An event invoked when all of the InputButton values required for this Bind to be triggered are released.
		/// Called immediately before Action is invoked.
		/// </summary>
		public event EventHandler Released;

		/// <summary>
		/// Gets or sets a value indicating whether this bind should be invoked every frame that the key is pressed, or only once until it's released.
		/// </summary>
		public bool MultiInvoke {
			get { return _MultiInvoke; }
			set { _MultiInvoke = value; }
		}

		/// <summary>
		/// Returns any InputButtons that will cause this Bind to be triggered when at least one of them is pressed (including modifiers).
		/// </summary>
		public IEnumerable<InputButton> Buttons {
			get { return _Buttons; }
		}

		/// <summary>
		/// Gets the InputManager that's handling this Bind.
		/// </summary>
		public InputManager InputManager {
			get { return _InputManager; }
		}

		/// <summary>
		/// Gets the action to be invoked when the bind is pressed.
		/// </summary>
		public Action<BindState> Action {
			get { return _Action; }
		}
		
		/// <summary>
		/// Gets the state of this bind.
		/// </summary>
		public BindState State {
			get { return _State; }
		}

		/// <summary>
		/// Creates a new instance of Bind that invokes the given Action when pressed.
		/// </summary>
		public Bind(InputManager InputManager, Action<BindState> Action, bool MultiInvoke, params InputButton[] Buttons) {
			this._InputManager = InputManager;
			this._Action = Action;
			this._MultiInvoke = MultiInvoke;
			foreach(var Button in Buttons)
				RegisterButton(Button);
		}

		/// <summary>
		/// Registers the given InputButton to cause this Bind to be triggered.
		/// </summary>
		public void RegisterButton(InputButton Button) {
			this._Buttons.Add(Button);
		}

		/// <summary>
		/// Unregisters the given InputButton from triggering this Bind.
		/// </summary>
		public void UnregisterButton(InputButton Button) {
			bool result = this._Buttons.Remove(Button);
			if(!result)
				throw new ArgumentException("Button was not associated with this Bind.");
		}

		/// <summary>
		/// Manually invokes this bind, raising the Pressed event and invoking the Action associated with it if Handled was false.
		/// </summary>
		internal void Invoke(BindState State) {
			this._State = State;
			var EventArgs = new BindPressEventArgs() {
				Action = this._Action,
				Handled = false,
				Player = InputManager.Player
			};
			if(State == BindState.Pressed) {
				if(this.Pressed != null)
					this.Pressed(this, EventArgs);
			} else if(State == BindState.Released) {
				if(this.Released != null)
					this.Released(this, EventArgs);
			}
			if(!EventArgs.Handled)
				this.Action(State);
		}
	}
}
