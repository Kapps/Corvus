using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CorvEngine.Input {

	/// <summary>
	/// Implements a tagged union representing a single button press, optionally with modifiers.
	/// Any input type handled by InputButton may be implicitly converted to an InputButton, but not the other way around.
	/// This struct is immutable.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct InputButton {
		/// <summary>
		/// Indicates the type of the input method that this InputButton is mapped to.
		/// </summary>
		[FieldOffset(0)]
		public readonly InputMethod Type;
		[FieldOffset(sizeof(int))]
		private readonly ConsoleModifiers _KeyModifiers;
		[FieldOffset(sizeof(int) + sizeof(ConsoleModifiers))]
		private readonly Buttons _Button;
		[FieldOffset(sizeof(int) + sizeof(ConsoleModifiers))]
		private readonly Keys _Key;

		/// <summary>
		/// Creates a new InputButton from the given key and, optionally, modifiers.
		/// </summary>
		public InputButton(Keys Key, ConsoleModifiers Modifiers = 0) {
			this._Button = 0; // Make it not complain about readonly being assigned, even though it's incorrect.
			this._Key = Key;
			this._KeyModifiers = Modifiers;
			this.Type = InputMethod.Key;
		}

		/// <summary>
		/// Creates a new InputButton from the given controller button.
		/// </summary>
		public InputButton(Buttons Button) {
			this._Key = 0; // Prevent complaining about readonly.
			this._KeyModifiers = 0;
			this._Button = Button;
			this.Type = InputMethod.Button;
		}

		/// <summary>
		/// Indicates any modifiers being used, if a Keyboard button is required.
		/// If Type is not set to Key, an InvalidInputException is thrown.
		/// If no modifiers are set, returns the enum value 0 (that is, an empty Flags enum).
		/// </summary>
		public ConsoleModifiers KeyModifiers {
			get {
				if(Type != InputMethod.Key)
					throw new InvalidInputException("Unable to access key modifiers when the ButtonType is not Key.");
				return _KeyModifiers;
			}
		}		
		
		/// <summary>
		/// Indicates the controller button this instance is mapped to, provided Type is set to Button.
		/// Otherwise, an InvalidInputException is thrown.
		/// </summary>
		public Buttons Button {
			get {
				if(Type != InputMethod.Button)
					throw new InvalidInputException("Unable to access Button when ButtonType is not Controller.");
				return _Button;
			}
		}
		/// <summary>
		/// Indicates the keyboard key this instance is mapped to, provided Type is set to Key.
		/// Otherwise, an InvalidInputException is thrown.
		/// </summary>
		public Keys Key {
			get {
				if(Type != InputMethod.Key)
					throw new InvalidInputException("Unable to access Key when the ButtonType is not Key.");
				return _Key;
			}
		}

		/// <summary>
		/// Implicitly converts a value of Keys into an InputButton instance.
		/// </summary>
		public static implicit operator InputButton(Keys Key) {
			return new InputButton(Key);
		}

		/// <summary>
		/// Implicitly converts a value of Buttons into an InputButton instance.
		/// </summary>
		public static implicit operator InputButton(Buttons Button) {
			return new InputButton(Button);
		}
	}
}
