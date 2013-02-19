using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Entities;
using CorvEngine.Input;
using Microsoft.Xna.Framework.Input;

namespace Corvus {
	/// <summary>
	/// Provides binds that are specific to Corvus.
	/// </summary>
	public class CorvusBinds {

		private List<Bind> _Binds;
		private Player _Player;

		/// <summary>
		/// Gets the binds that are associated with this instance.
		/// </summary>
		public IEnumerable<Bind> Binds {
			get { return _Binds; }
		}

		private MovementComponent MovementComponent {
			get { return _Player.Character == null ? null : _Player.Character.GetComponent<MovementComponent>(); }
		}

		private CorvusBinds(Player Player) {
			this._Player = Player;
			this._Binds = new List<Bind>();

			switch(_Player.Index) {
				case 1:
					Assign(JumpPressed, false, new InputButton(Keys.Space));
					Assign((c) => MovePressed(Direction.Left, c), true, new InputButton(Keys.Left));
					Assign((c) => MovePressed(Direction.Right, c), true, new InputButton(Keys.Right));
					break;
			}
		}

		/// <summary>
		/// Creates and returns the binds for the specified player index.
		/// </summary>
		/// <param name="PlayerIndex">The one-based index of the player.</param>
		public static IEnumerable<Bind> CreateBinds(Player Player) {
			return new CorvusBinds(Player)._Binds;
		}

		private void Assign(Action<BindState> Action, bool MultiInvoke, params InputButton[] Buttons) {
			Bind Bind = new Bind(_Player.InputManager, Action, MultiInvoke);
			foreach(var Button in Buttons) {
				Bind.RegisterButton(Button);
			}
			_Binds.Add(Bind);
			_Player.InputManager.RegisterBind(Bind);
		}

		private void MovePressed(Direction Direction, BindState State) {
			// TODO: Fix this when MovementComponent makes sense.
			if(MovementComponent == null)
				return;
			switch(State) {
				case BindState.Pressed:
					if(WalkAction != null)
						CorvusGame.Instance.FrameInvoker.RemoveCommand(WalkAction);
					WalkAction = new Action(() => {
						MovementComponent.Walk(Direction);
					});
					CorvusGame.Instance.FrameInvoker.RegisterCommand(WalkAction);
					break;
				case BindState.Released:
					if(WalkAction != null)
						CorvusGame.Instance.FrameInvoker.RemoveCommand(WalkAction);
					MovementComponent.Walk(CorvEngine.Direction.None);
					WalkAction = null;
					break;
			}
		}

		private void JumpPressed(BindState State) {
			if(MovementComponent == null)
				return;
			if(State == BindState.Pressed)
				MovementComponent.StartJump(true);
			//else
				//MovementComponent.EndStartJump();
		}


		private Action WalkAction;
	}
}
