using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;
using CorvEngine.Components;
namespace Corvus.Components.Gameplay.StatusEffects
{
    //TODO: Probably could do some refactoring here. For example, move _TickTimer into this class so every class that
    //      Inherits from this will have access to it. 

    /// <summary>
    /// A base class for a status effect.
    /// </summary>
    /// <remarks>
    /// I would've preferred to call this class Effect but there is already a class that microsoft created called that.
    /// </remarks>
    public abstract class StatusEffect
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
        /// Gets a value determining whether this effect's duration has ended.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Gets the entity being affected.
        /// </summary>
        public Entity Entity { get { return _Entity; } }

        /// <summary>
        /// A event that occurs when the status effect is finished. 
        /// </summary>
        protected event EventHandler OnEventCompleted;

        private float _BaseValue;
        private float _Intensity;
        private float _Duration;
        private Entity _Entity;
        protected Sprite _Sprite;
        protected TimeSpan _Timer = TimeSpan.Zero;
        protected FloatingTextList _FloatingTexts = new FloatingTextList();
        
        /// <summary>
        /// Creates a new instance of StatusEffect.
        /// </summary>
        /// <param name="entity">The entity being affected.</param>
        public StatusEffect(Entity entity)
        {
            this._Sprite = CorvusGame.Instance.GlobalContent.LoadSprite(SpriteName);
            this._Entity = entity;
        }

        //TODO: Find a better way to draw effects
        public virtual void Update(GameTime gameTime)
        {
            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(Duration))
            {
                IsFinished = true;
                if (OnEventCompleted != null)
                    OnEventCompleted(this, new EventArgs());
            }
            _Sprite.ActiveAnimation.AdvanceAnimation(gameTime.ElapsedGameTime);

            var width = Entity.Size.X;
            var position = Entity.Position + new Vector2(width / 2, 0);
            var ToScreen = Camera.Active.ScreenToWorld(position);
            _FloatingTexts.Update(gameTime, ToScreen);
        }

        //TODO: Maybe make the effect scale with the size of entity. 
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
