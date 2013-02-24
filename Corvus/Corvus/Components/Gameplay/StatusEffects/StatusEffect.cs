using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components.Gameplay.StatusEffects
{
    /// <summary>
    /// A base class for a status effect.
    /// </summary>
    public abstract class StatusEffect
    {
        /// <summary>
        /// Gets the name of the status effect.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the image to display for this status effect.
        /// </summary>
        protected abstract Texture2D EffectImage { get; }

        /// <summary>
        /// Gets or sets intensity of the effect. 
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
            set { _Duration = value; }
        }

        /// <summary>
        /// Gets a value determining whether this effect's duration has ended.
        /// </summary>
        public bool IsFinished { get; private set; }

        private float _Intensity;
        private float _Duration;
        protected TimeSpan _Timer = TimeSpan.Zero;
        protected FloatingTextList _FloatingTexts = new FloatingTextList();

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void ResetTimer()
        {
            _Timer = TimeSpan.Zero;
        }

        public virtual void Update(GameTime gameTime, AttributesComponent ac, Vector2 position)
        {
            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(Duration))
                IsFinished = true;
            _FloatingTexts.Update(gameTime, position);
        }

        public virtual void Draw()
        {
            //TODO: Draw the image effect here.
            _FloatingTexts.Draw();
        }
    }
}
