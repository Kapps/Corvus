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
        
        private PlayerControlComponent PlayerControlComponent {
			get { return _Player.Character == null ? null : _Player.Character.GetComponent<PlayerControlComponent>(); }
		}

		private CorvusBinds(Player Player) {
			this._Player = Player;
			this._Binds = new List<Bind>();

			switch(_Player.Index) {
				case 1:
                    //keyboard
                    //menu
                    Assign((c) => MainMenuNavigation(true, c), false, new InputButton(Keys.Up));
                    Assign((c) => MainMenuNavigation(false, c), false, new InputButton(Keys.Down));
                    Assign(MainMenuSelect, false, new InputButton(Keys.Enter));
                    //ingame
					Assign(c => JumpPressed(false, c), false, new InputButton(Keys.Space));
					Assign((c) => MovePressed(Direction.Left, c), false, new InputButton(Keys.Left));
					Assign((c) => MovePressed(Direction.Right, c), false, new InputButton(Keys.Right));
                    Assign(BlockPressed, false, new InputButton(Keys.X));
                    Assign((c) => SwitchWeapon(true, c), false, new InputButton(Keys.A));
                    Assign((c) => SwitchWeapon(false, c), false, new InputButton(Keys.S));
                    Assign(Attack, false, new InputButton(Keys.Z));
                    Assign(Pause, false, new InputButton(Keys.Escape));
                    //paused
                    Assign((c) => PausedNavigation(true, c), false, new InputButton(Keys.Up));
                    Assign((c) => PausedNavigation(false, c), false, new InputButton(Keys.Down));
                    Assign(PausedSelect, false, new InputButton(Keys.Enter));
                    //xbox controller
                    //menu
                    Assign((c) => MainMenuNavigation(true, c), false, new InputButton(Buttons.LeftThumbstickUp));
                    Assign((c) => MainMenuNavigation(false, c), false, new InputButton(Buttons.LeftThumbstickDown));
                    Assign(MainMenuSelect, false, new InputButton(Buttons.A));
                    //ingame
                    Assign(c => JumpPressed(false, c), false, new InputButton(Buttons.A));
                    Assign((c) => MovePressed(Direction.Left, c), false, new InputButton(Buttons.LeftThumbstickLeft));
                    Assign((c) => MovePressed(Direction.Right, c), false, new InputButton(Buttons.LeftThumbstickRight));
                    Assign(BlockPressed, false, new InputButton(Buttons.RightTrigger));
                    Assign((c) => SwitchWeapon(true, c), false, new InputButton(Buttons.RightShoulder));
                    Assign((c) => SwitchWeapon(false, c), false, new InputButton(Buttons.LeftShoulder));
                    Assign((c) => SwitchWeapon(true, c), false, new InputButton(Buttons.X));
                    Assign((c) => SwitchWeapon(false, c), false, new InputButton(Buttons.Y));
                    Assign(Attack, false, new InputButton(Buttons.B));
                    Assign(Pause, false, new InputButton(Buttons.Start));
                    //paused
                    Assign((c) => PausedNavigation(true, c), false, new InputButton(Buttons.LeftThumbstickUp));
                    Assign((c) => PausedNavigation(false, c), false, new InputButton(Buttons.LeftThumbstickDown));
                    Assign(PausedSelect, false, new InputButton(Buttons.A));
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
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.SceneManager)
                return;
			switch(State) {
				case BindState.Pressed:
                    PlayerControlComponent.BeginWalking(Direction);
					break;
				case BindState.Released:
                    if (PlayerControlComponent.CurrentDirection == Direction)
                        PlayerControlComponent.StopWalking();
					break;
			}
		}

		private void JumpPressed(bool isMultijump, BindState State) {
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.SceneManager)
                return;
			if(State == BindState.Pressed)
                PlayerControlComponent.Jump(isMultijump);
		}
        
        private void BlockPressed(BindState State){
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.SceneManager)
                return;
            switch (State)
            {
                case BindState.Pressed:
                    PlayerControlComponent.BeginBlock();
                    break;
                case BindState.Released:
                    PlayerControlComponent.EndBlock();
                    break;
            }
        }

        private void SwitchWeapon(bool isPrev, BindState state){
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.SceneManager)
                return;
            if (state == BindState.Pressed)
            {
                if (isPrev)
                    PlayerControlComponent.SwitchWeapon(isPrev);
                else
                    PlayerControlComponent.SwitchWeapon(isPrev);
            }
        }

        private void Attack(BindState State){
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.SceneManager)
                return;
            if (State == BindState.Pressed)
                PlayerControlComponent.StartAttack();
            else if (State == BindState.Released)
                PlayerControlComponent.EndAttack();
        }

        private void Pause(BindState state)
        {
            if (CorvusGame.Instance.StateManager.GetCurrentState() == CorvusGame.Instance.MainMenuState ||
                CorvusGame.Instance.StateManager.GetCurrentState() == CorvusGame.Instance.PausedState)
                return;
            if (state == BindState.Pressed)
                CorvusGame.Instance.StateManager.PushState(CorvusGame.Instance.PausedState);

            GamepadComponent.StopVibrations();
        }

        private void MainMenuNavigation(bool isPrev, BindState state)
        {
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.MainMenuState)
                return;
            if (state == BindState.Pressed)
            {
                if (isPrev)
                    CorvusGame.Instance.MainMenuState.ControlManager.PreviousControl();
                else
                    CorvusGame.Instance.MainMenuState.ControlManager.NextControl();
            }
        }

        private void MainMenuSelect(BindState state)
        {
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.MainMenuState)
                return;
            if (state == BindState.Pressed)
                CorvusGame.Instance.MainMenuState.ControlManager.SelectControl();
        }

        private void PausedNavigation(bool isPrev, BindState state)
        {
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.PausedState)
                return;
            if (state == BindState.Pressed)
            {
                if (isPrev)
                    CorvusGame.Instance.PausedState.ControlManager.PreviousControl();
                else
                    CorvusGame.Instance.PausedState.ControlManager.NextControl();
            }
        }

        private void PausedSelect(BindState state)
        {
            if (CorvusGame.Instance.StateManager.GetCurrentState() != CorvusGame.Instance.PausedState)
                return;
            if (state == BindState.Pressed)
                CorvusGame.Instance.PausedState.ControlManager.SelectControl();
        }
	}
}

/*


        private void MenuBinds()
        {
            var player = this.Players.First();

            Bind Next = new Bind(player.InputManager, _MainMenuState.ControlManager.NextControl, false, Keys.Down);
            player.InputManager.RegisterBind(Next);
            Bind Prev = new Bind(player.InputManager, _MainMenuState.ControlManager.PreviousControl, false, Keys.Up);
            player.InputManager.RegisterBind(Prev);
            Bind Select = new Bind(player.InputManager, _MainMenuState.ControlManager.SelectControl, false, Keys.Enter);
            player.InputManager.RegisterBind(Select);
            //xbox controller
            Bind GamePadNext = new Bind(player.InputManager, _MainMenuState.ControlManager.NextControl, false, Buttons.LeftThumbstickDown);
            player.InputManager.RegisterBind(GamePadNext);
            Bind GamePadPrev = new Bind(player.InputManager, _MainMenuState.ControlManager.PreviousControl, false, Buttons.LeftThumbstickUp);
            player.InputManager.RegisterBind(GamePadPrev);
            Bind GamePadSelect = new Bind(player.InputManager, _MainMenuState.ControlManager.SelectControl, false, Buttons.A);
            player.InputManager.RegisterBind(GamePadSelect);
        }

*/