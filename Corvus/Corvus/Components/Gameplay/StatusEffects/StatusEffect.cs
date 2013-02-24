using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;

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
        /// Gets the sprite name to display for this effect.
        /// </summary>
        protected abstract string EffectName { get; }

        /// <summary>
        /// Gets or sets the static amount this status effect should inflict. 
        /// </summary>
        public float BaseEffect
        {
            get { return _BaseEffect; }
            set { _BaseEffect = Math.Max(value, 0); }
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

        private Sprite _EffectSprite;
        private float _BaseEffect;
        private float _Intensity;
        private float _Duration;
        private Vector2 _EntitySize;
        protected TimeSpan _Timer = TimeSpan.Zero;
        protected FloatingTextList _FloatingTexts = new FloatingTextList();
        protected Vector2 Position { get; set; }

        public StatusEffect()
        {
            _EffectSprite = CorvusGame.Instance.GlobalContent.LoadSprite(EffectName);
        }
        
        public virtual void Update(GameTime gameTime, AttributesComponent ac, Vector2 position)
        {
            Position = position;
            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(Duration))
                IsFinished = true;
            _EffectSprite.ActiveAnimation.AdvanceAnimation(gameTime.ElapsedGameTime);
            _FloatingTexts.Update(gameTime, position);
        }

        //TODO: Maybe make the effect scale with the size of entity. 
        //TODO: Instead of drawing it here, maybe force every StatusEffect to draw it's own effect with it's own specialized position, etc.
        public virtual void Draw()
        {
            var ActiveFrame = _EffectSprite.ActiveAnimation.ActiveFrame.Frame;
            var SourceRect = ActiveFrame.Source;
            var destinationRect = new Rectangle((int)(Position.X - EntitySize.X / 2 + SourceRect.Width/2), (int)(Position.Y - EntitySize.Y), SourceRect.Width, SourceRect.Height);
            CorvusGame.Instance.SpriteBatch.Draw(_EffectSprite.Texture, destinationRect, SourceRect, Color.White);
		    _FloatingTexts.Draw();
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void ResetTimer()
        {
            _Timer = TimeSpan.Zero;
        }
    }
}
