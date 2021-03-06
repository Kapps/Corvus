﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using CorvEngine.Input;

namespace CorvEngine {
	/// <summary>
	/// Indicates one of the players in the game. A player may or may not be in the game, and they may or may not be controlling a character at this time.
	/// </summary>
	public class Player {

		/// <summary>
		/// Called when the character for this Player is changed with the new Entity that it was changed to.
		/// </summary>
		public event Action<Entity> CharacterChanged;

		/// <summary>
		/// Gets the InputManager being used for this player.
		/// </summary>
		public InputManager InputManager { get; private set; }

		/// <summary>
		/// Gets the Camera that's currently being used for this Player.
		/// </summary>
		public Camera Camera { get; private set; }

		/// <summary>
		/// Gets or sets the character that this player is controlling.
		/// </summary>
		public Entity Character {
			get { return _Character; }
			set {
				if(_Character == value)
					return;
				_Character = value;
				if(CharacterChanged != null)
					CharacterChanged(_Character);
			}
		}
		/// <summary>
		/// Creates a new Player with an InputManager attached to it.
		/// </summary>
		public Player(Entity Character) {
			this.InputManager = new InputManager(this);
			this.Camera = new Camera();
			this.Character = Character;
		}

		/// <summary>
		/// Gets the index of this player, from 1 to N where N is the number of players in the game.
		/// If the player is not registered with the game engine, the result is -1.
		/// </summary>
		public int Index {
			get { 
				// TODO: What a hacky implementation.
				// But we want the game itself to create the Player, so they can substitute their own Player class that handles things like what their character is and such.
				// And we don't want the game to manually manage player indexes.
				int Result = CorvBase.Instance.Players.ToList().IndexOf(this);
				return Result >= 0 ? Result + 1 : -1;
			}
		}

		private Entity _Character;
	}
}
