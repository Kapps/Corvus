using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CorvEngine.Input {

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
	/// Provides a global component that handles for input associated with a given player and either controller or keyboard.
	/// </summary>
	public class InputManager : GameComponent {
		/// <summary>
		/// Gets the player that this InputManager handles input for.
		/// </summary>
		public Player Player { get; private set; }
		
		/// <summary>
		/// Creates a new InputManager for the given Player.
		/// </summary>
		public InputManager(Player Player) : base(CorvBase.Instance.Game) {
			this.Player = Player;
			this.PreviousGS = GamePad.GetState(PlayerIndex.One);
			this.PreviousKS = Keyboard.GetState();
		}

		/// <summary>
		/// Gets all of the active binds for this InputManager.
		/// </summary>
		public IEnumerable<Bind> Binds {
			get { return _Binds; }
		}

		/// <summary>
		/// Adds the specified bind to be managed by this InputManager.
		/// </summary>
		public void RegisterBind(Bind Bind) {
			this._Binds.Add(Bind);
		}

		/// <summary>
		/// Removes the specified bind from being managed by this InputManager.
		/// </summary>
		public void RemoveBind(Bind Bind) {
			bool Result = this._Binds.Remove(Bind);
			if(!Result)
				throw new KeyNotFoundException();
		}

		public override void Update(GameTime gameTime) {
			KeyboardState KS = Keyboard.GetState();
			GamePadState GS = GamePad.GetState(PlayerIndex.One + (this.Player.Index - 1));
			foreach(var Bind in this._Binds) {
				foreach(var Button in Bind.Buttons) {
					bool PressedNow = IsButtonPressed(Button, KS, GS);
					bool PressedBefore = IsButtonPressed(Button, PreviousKS, PreviousGS);
					if(PressedNow && (!PressedBefore || Bind.MultiInvoke))
						Bind.Invoke(BindState.Pressed);
					else if(!PressedNow && PressedBefore)
						Bind.Invoke(BindState.Released);
				}
			}
			PreviousKS = KS;
			PreviousGS = GS;
			base.Update(gameTime);
		}

		private bool IsButtonPressed(InputButton Button, KeyboardState KS, GamePadState GS) {
			switch(Button.Type) {
				case InputMethod.Key:
					return KS.IsKeyDown(Button.Key);
				case InputMethod.Button:
					return GS.IsButtonDown(Button.Button);
				default:
					throw new ArgumentOutOfRangeException("Button.Type");
			}
		}

		private KeyboardState PreviousKS;
		private GamePadState PreviousGS;
		private List<Bind> _Binds = new List<Bind>();
		// TODO: There should be something related to GameState here.
		// It makes no sense that you should keep your binds for attack and such when you're in a different state.
		// Maybe give Bind an option to trigger only if a specified GameState is not blocked for updates.
	}
}
