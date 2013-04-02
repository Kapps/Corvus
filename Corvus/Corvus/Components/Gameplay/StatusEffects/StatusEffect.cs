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
        /// Gets the name of the status effect. Also determines the effect to use.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the sprite sheet to get the animation from.
        /// </summary>
        protected abstract string SpriteName { get; }

        /// <summary>
        /// Gets a value that indicates this is a 'good' effect. (Ex: healing is good, poison is bad).
        /// </summary>
        protected abstract bool IsGood { get; }

        /// <summary>
        /// Gets the attributes for this effect.
        /// </summary>
        public StatusEffectProperties Attributes { get { return _Attributes; } }

        /// <summary>
        /// Gets a value determining whether this effect's duration has ended.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Gets whether this effect is positive or negative.
        /// </summary>
        public bool IsPositive { get { return IsGood; } }

        /// <summary>
        /// Gets the entity being affected.
        /// </summary>
        public Entity Entity { get { return _Entity; } }
        private Color ImageColor { get { return (!IsGood) ? Color.LightBlue : Color.LightSalmon; } }

        private StatusEffectProperties _Attributes;
        private Entity _Entity;
        protected Sprite _Sprite;
        protected TimeSpan _Timer = TimeSpan.Zero;
        protected TimeSpan _TickTimer = TimeSpan.Zero;
        protected TimeSpan _TickOccurance = TimeSpan.FromSeconds(1); //how long before a tick is registered.
        protected bool _IsFirstOccurance = true;
        protected FloatingTextComponent FloatingTextComponent;

        /// <summary>
        /// Creates a new instance of StatusEffect.
        /// </summary>
        /// <param name="entity">The entity being affected.</param>
        public StatusEffect(Entity entity, StatusEffectProperties attributes)
        {
            var effect = CorvusGame.Instance.GlobalContent.LoadSprite(SpriteName); 
            var animation = effect.Animations.First();
            effect.PlayAnimation(animation.Name);
            this._Sprite = effect;
            this._Entity = entity;
            this._Attributes = attributes;
            FloatingTextComponent = entity.GetComponent<FloatingTextComponent>();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_IsFirstOccurance)
            {
                OnFirstOccurance();
                _IsFirstOccurance = false;
            }

            _TickTimer += gameTime.ElapsedGameTime;
            if (_TickTimer >= _TickOccurance)
            {
                OnTick();
                _TickTimer = TimeSpan.Zero;
            }

            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(Attributes.Duration))
            {
                IsFinished = true;
                OnFinished();
            }
            _Sprite.ActiveAnimation.AdvanceAnimation(gameTime.ElapsedGameTime);
        }

        public virtual void Draw(Vector2 position)
        {
            var SourceRect = _Sprite.ActiveAnimation.ActiveFrame.Frame.Source;
            CorvusGame.Instance.SpriteBatch.Draw(_Sprite.Texture, position, SourceRect, ImageColor);
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void ResetTimer()
        {
            _Timer = TimeSpan.Zero;
        }

        /// <summary>
        /// Fires at the very start of the status effect. Use for one time events.
        /// </summary>
        protected abstract void OnFirstOccurance();

        /// <summary>
        /// Fires every time a tick is registered.
        /// </summary>
        protected abstract void OnTick();

        /// <summary>
        /// Fires when the effect has finished.
        /// </summary>
        protected abstract void OnFinished();

        /// <summary>
        /// Forces the statuse effect to set call it's OnFinished function.
        /// </summary>
        public void ForceFinish()
        {
            OnFinished();
        }

    }
}
