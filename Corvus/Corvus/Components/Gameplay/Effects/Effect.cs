using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;

namespace Corvus.Components.Gameplay.Effects
{
    /// <summary>
    /// A base class for a status effect.
    /// </summary>
    public abstract class Effect
    {
        /// <summary>
        /// Gets the name of the status effect.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the sprite name to display for this effect.
        /// </summary>
        protected abstract string SpriteName { get; }

        /// <summary>
        /// Gets or sets the static amount this status effect should inflict. 
        /// </summary>
        public float BaseValue
        {
            get { return _BaseValue; }
            set { _BaseValue = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets intensity of the effect. This is usually expressed as a percentage. 
        /// </summary>
        public float Intensity
        {
            get { return _Intensity; }
            set { _Intensity = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the duration of the effect in seconds.
        /// </summary>
        public float Duration
        {
            get { return _Duration; }
            set { _Duration = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the entity size so it knows where to draw the effect.
        /// </summary>
        public Vector2 EntitySize 
        {
            get { return _EntitySize; }
            set { _EntitySize = value; }
        }

        /// <summary>
        /// Gets a value determining whether this effect's duration has ended.
        /// </summary>
        public bool IsFinished { get; private set; }

        private float _BaseValue;
        private float _Intensity;
        private float _Duration;
        private Vector2 _EntitySize;
        protected Sprite _Sprite;
        protected TimeSpan _Timer = TimeSpan.Zero;
        protected FloatingTextList _FloatingTexts = new FloatingTextList();
        protected Vector2 Position { get; set; }

        public Effect()
        {
            _Sprite = CorvusGame.Instance.GlobalContent.LoadSprite(SpriteName);
        }
        
        public virtual void Update(GameTime gameTime, AttributesComponent ac, Vector2 position)
        {
            Position = position;
            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(Duration))
                IsFinished = true;
            _Sprite.ActiveAnimation.AdvanceAnimation(gameTime.ElapsedGameTime);
            _FloatingTexts.Update(gameTime, position);
        }

        public abstract void Draw();

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void ResetTimer()
        {
            _Timer = TimeSpan.Zero;
        }
    }
}
