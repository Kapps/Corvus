using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Components;
using CorvEngine.Input;
using Corvus.Components;
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

		private CombatComponent CombatComponent {
			get { return _Player.Character == null ? null : _Player.Character.GetComponent<CombatComponent>(); }
		}

		private CorvusBinds(Player Player) {
			this._Player = Player;
			this._Binds = new List<Bind>();

			switch(_Player.Index) {
				case 1:
					Assign(JumpPressed, false, new InputButton(Keys.Space));
					Assign((c) => MovePressed(Direction.Left, c), false, new InputButton(Keys.Left));
					Assign((c) => MovePressed(Direction.Right, c), false, new InputButton(Keys.Right));
					Assign(AttackSwordPressed, false, new InputButton(Keys.Z));
                    Assign(AttackGunPressed, false, new InputButton(Keys.X));
                    Assign(BlockPressed, false, new InputButton(Keys.C));
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
					MovementComponent.BeginWalking(Direction);
					break;
				case BindState.Released:
					if(MovementComponent.CurrentDirection == Direction)
						MovementComponent.StopWalking();
					break;
			}
		}

		private void JumpPressed(BindState State) {
			if(MovementComponent == null)
				return;
			if(State == BindState.Pressed)
				MovementComponent.Jump(true);
		}

		private void AttackSwordPressed(BindState State) {
			if(MovementComponent == null)
				return;
			if(State == BindState.Pressed)
				CombatComponent.AttackMelee();
		}

        private void AttackGunPressed(BindState State)
        {
            if (MovementComponent == null)
                return;
            if (State == BindState.Pressed)
                CombatComponent.AttackRanged();
        }

        private void BlockPressed(BindState State)
        {
            if (MovementComponent == null)
                return;
            switch (State)
            {
                case BindState.Pressed:
                    CombatComponent.BeginBlock();
                    break;
                case BindState.Released:
                    CombatComponent.EndBlock();
                    break;
            }
        }
	}
}
