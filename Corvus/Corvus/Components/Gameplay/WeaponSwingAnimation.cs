using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;
using CorvEngine.Graphics;
using CorvEngine.Components;

namespace Corvus.Components.Gameplay
{
    /// <summary>
    /// A class to animate a weapon. More like make it rotate 90 degrees.
    /// </summary>
    public class WeaponSwingAnimation
    {
        private Texture2D _Weapon;
        private Entity _Entity;
        private float _Duration = 0f;
        private float _RotationAngle;
        private Vector2 _OffSet = new Vector2();
        private Vector2 _Origin;
        private SpriteEffects _Flip;
        private DateTime _StartTime;
        private float _Direction;
        private bool _StartAnimation = false;

        /// <summary>
        /// Starts the weapon swing animation based on the entity, weapon name, and it's duration (usually the attack speed.).
        /// </summary>
        public void Start(Entity src, string weapon, float duration, Vector2 offset)
        {
            _StartAnimation = true;
            _Entity = src;
            _Weapon = CorvusGame.Instance.GlobalContent.Load<Texture2D>(weapon);
            _Duration = duration;
            _StartTime = DateTime.Now;
            _RotationAngle = 0f;

            var mc = _Entity.GetComponent<MovementComponent>();
            _Direction = CorvusExtensions.GetSign(mc.CurrentDirection);
            _Origin = (mc.CurrentDirection == Direction.Left) ? new Vector2(_Entity.Size.X / 2, _Entity.Size.Y / 2) : new Vector2(0f, _Entity.Size.Y / 2);
            _Flip = (mc.CurrentDirection == Direction.Left) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _OffSet = new Vector2(_Direction * offset.X, offset.Y);
        }

        /// <summary>
        /// Force the animation to stop.
        /// </summary>
        public void Stop()
        {
            _StartAnimation = false;
        }


        public void Update(GameTime Time)
        {
            if (!_StartAnimation)
                return;
            if ((DateTime.Now - _StartTime).TotalMilliseconds >= _Duration)
            {
                _StartAnimation = false;
                return;
            }
            _RotationAngle += (float)((Time.ElapsedGameTime.TotalSeconds) * _Direction) * ((MathHelper.PiOver2) / (_Duration / 1000)); 
        }

        public void Draw()
        {
            if (!_StartAnimation)
                return;

            var pos = new Vector2(_Entity.Location.Center.X + _Direction * _Entity.Size.X / 2 + _OffSet.X, _Entity.Location.Center.Y + _OffSet.Y);
            var toscreen = Camera.Active.WorldToScreen(pos);
            CorvusGame.Instance.SpriteBatch.Draw(_Weapon, toscreen, new Rectangle(0,0,22,22), Color.White, _RotationAngle, _Origin, 1f, _Flip, 0f);
        }


    }
}
