using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Corvus.GameStates;
using Corvus;

namespace Corvus.Components
{
    /// <summary>
    /// A player only component to manage gamepad specific functionality.... mainly vibrations at the moment.
    /// </summary>
    public class GamepadComponent : Component
    {
        private Player _Player;
        private bool _IsVibrating = false;
        private float _Duration = 0f;
        private TimeSpan _Timer = TimeSpan.Zero;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            //Get player.
            foreach (var p in CorvusGame.Instance.Players)
            {
                if (this.Parent.ID == p.Character.ID)
                {
                    _Player = p;
                    break;
                }
            }
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            if (_IsVibrating)
            {
                _Timer += Time.ElapsedGameTime;
                if (_Timer >= TimeSpan.FromSeconds(_Duration))
                {
                    GamePad.SetVibration(DetermineIndex(_Player.Index), 0f, 0f);
                    _IsVibrating = false;
                    _Timer = TimeSpan.Zero;
                }
            }
        }

        private PlayerIndex DetermineIndex(int index)
        {
            switch (index)
            {
                case 1: return PlayerIndex.One;
                case 2: return PlayerIndex.Two;
                case 3: return PlayerIndex.Three;
                case 4: return PlayerIndex.Four;
                default: return PlayerIndex.One;
            }
        }

        protected override void OnDispose()
        {
            if (_IsVibrating)
            {
                GamePad.SetVibration(DetermineIndex(_Player.Index), 0f, 0f);
                _IsVibrating = false;
                _Timer = TimeSpan.Zero;
            }
            base.OnDispose();
        }

        /// <summary>
        /// Vibrates the controller based on the specified motors and duration. Left is slow vibrations, right is faster vibrations.
        /// </summary>
        public static void Vibrate(Entity player, float leftMotor, float rightMotor, float duration)
        {
            if (player.IsDisposed)
                return;
            var gc = player.GetComponent<GamepadComponent>();
            if (gc == null)
                return;
            if (gc._IsVibrating)
            {
                gc._Timer = TimeSpan.Zero;
                return;
            }
            gc._IsVibrating = true;
            GamePad.SetVibration(gc.DetermineIndex(gc._Player.Index), MathHelper.Clamp(leftMotor, 0f, 1f), MathHelper.Clamp(rightMotor, 0f, 1f));
            gc._Duration = duration;
        }
    }
}
