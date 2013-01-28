﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine {

	/// <summary>
	/// Indicates what input method is being used; either a keyboard, or a controller.
	/// </summary>
	public enum InputMethod : int {
		/// <summary>
		/// No input method has been set.
		/// </summary>
		None = 0,
		/// <summary>
		/// The input method is a keyboard.
		/// </summary>
		Key = 1,
		/// <summary>
		/// The input method is a controller.
		/// </summary>
		Button = 2,
	}

	/// <summary>
	/// Provides a handler for input, associated with a given player and either controller or keyboard.
	/// </summary>
	public class InputManager {
		/// <summary>
		/// Gets the player that this InputManager handles input for.
		/// </summary>
		public Player Player { get; private set; }
		
		/// <summary>
		/// Creates a new InputManager for the given Player.
		/// </summary>
		public InputManager(Player Player) {
			this.Player = Player;
		}
	}
}
