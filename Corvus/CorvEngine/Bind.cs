using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine {

	/// <summary>
	/// Provides an EventArgs value for when a bind is pressed, optionally preventing the action from being invoked.
	/// </summary>
	public class BindPressEventArgs : EventArgs {
		/// <summary>
		/// Indicates whether this event has been handled, preventing the mapped action from being invoked.
		/// </summary>
		public bool Handled { get; set; }

		/// <summary>
		/// Gets the action that should be invoked for this press.
		/// </summary>
		public Action Action { get; set; }

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
		private Action _Action;
		private InputManager _InputManager;

		/// <summary>
		/// An event invoked when one or more of the InputButton values required for this Bind to be triggered are pressed, immediately before Action is invoked.
		/// </summary>
		public event EventHandler Pressed;

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
		public Action Action {
			get { return _Action; }
		}

		/// <summary>
		/// Creates a new instance of Bind that invokes the given Action when pressed.
		/// </summary>
		public Bind(InputManager InputManager, Action Action) {
			this._InputManager = InputManager;
			this._Action = Action;
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
		internal void Invoke() {
			var EventArgs = new BindPressEventArgs() {
				Action = this._Action,
				Handled = false,
				Player = InputManager.Player
			};
			if(this.Pressed != null)
				this.Pressed(this, EventArgs);
			if(!EventArgs.Handled)
				this.Action();
		}
	}
}
