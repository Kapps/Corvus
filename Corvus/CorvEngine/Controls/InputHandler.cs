using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CorvEngine.Controls
{
    /// <summary>
    /// Temp class until Controls are added.
    /// </summary>
    public class InputHandler : GameComponent
    {
        #region Contructor 
        public InputHandler(Game game)
            : base(game)
        {
            keyboardState = Keyboard.GetState();
            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            previousGamePadStates = (GamePadState[])gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);
            base.Update(gameTime);
        }
        #endregion

        #region Keyboard

        #region Fields/Properties
        private static KeyboardState keyboardState;
        public static KeyboardState KeyboardState { get { return keyboardState; } }

        private static KeyboardState previousKeyboardState;
        public static KeyboardState PreviousKeyboardState { get { return previousKeyboardState; } }
        #endregion

        #region Methods
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        #endregion

        #endregion

        #region GamePad

        #region Fields/Properties
        private static GamePadState[] gamePadStates;
        public static GamePadState[] GamePadStates { get { return gamePadStates; } }

        private static GamePadState[] previousGamePadStates;
        public static GamePadState[] PreviousGamePadStates { get { return previousGamePadStates; } }
        #endregion

        #region Methods
        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) && previousGamePadStates[(int)index].IsButtonDown(button);
        }
        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) && previousGamePadStates[(int)index].IsButtonUp(button);
        }
        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }
        #endregion

        #endregion

    }
}
