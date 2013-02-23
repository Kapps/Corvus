using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components
{
    /// <summary>
    /// Hopefully temp class. Right now, it is here so the BluePrintParser can read which type of Status Effect it is.
    /// </summary>
    public enum StatusEffectTypes
    {
        Poison
    }

    /// <summary>
    /// A Component to handle status effects applied to this entity.
    /// </summary>
    public class StatusEffectsComponent : Component
    {
        private AttributesComponent AttributesComponent;
        private StatusEffectCollection _StatusEffects = new StatusEffectCollection();

        /// <summary>
        /// A plays a status effect.
        /// </summary>
        /// <param name="type">The type of status effect.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">How long the effect should last.</param>
        public void ApplyStatusEffect(Type type, float intensity, float duration)
        {
            var statusEffect = (StatusEffect)Activator.CreateInstance(type);
            statusEffect.Intensity = intensity;
            statusEffect.Duration = duration;
            _StatusEffects.Add(statusEffect);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            if (_StatusEffects.Count == 0)
                return;
            var width = Parent.Size.X;
            var position = Parent.Position + new Vector2(width / 2, 0);
            var ToScreen = Camera.Active.ScreenToWorld(position);
            foreach (StatusEffect se in _StatusEffects.Reverse<StatusEffect>())
            {
                se.Update(Time, AttributesComponent, ToScreen);
                if (se.IsFinished)
                    _StatusEffects.Remove(se);
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();
            if (_StatusEffects.Count == 0)
                return;
            foreach (StatusEffect se in _StatusEffects)
                se.Draw();
        }
    }

    //TODO: Refactor. Or let Onion do it since i'm not sure how he wants things.

    /// <summary>
    /// A collection of StatusEffects.
    /// </summary>
    public class StatusEffectCollection : KeyedCollection<string, StatusEffect>
    {
        protected override string GetKeyForItem(StatusEffect item)
        {
            return item.Name;
        }
        
        /// <summary>
        /// We want to only add it if that type of status effect doesn't already exist. Otherwise update the existing one.
        /// </summary>
        protected override void InsertItem(int index, StatusEffect item)
        {
            if (!this.Contains(item.Name))
                base.InsertItem(index, item);
            else
            {
                //Update the existing one, if it's better.
                var effect = this[item.Name];
                if (effect.Intensity < item.Intensity)
                {
                    //new effect is better, apply it's properties
                    effect.Intensity = item.Intensity;
                    effect.Duration = item.Duration;
                }
                //Set the duration if it's longer.
                else if(effect.Duration < item.Duration)
                    effect.Duration = item.Duration;
                effect.ResetTimer(); 
            }   
        }
    }

    //TODO: Maybe create a seperate class that handles all damage over time effects and another that
    //      handles all the one time only effects (Ex: strength down.)

    /// <summary>
    /// A status effect that causes damage over time.
    /// </summary>
    public class Poison : StatusEffect
    {
        //TODO: Set the image to draw
        
        public override string Name { get { return "Poison"; } }
        protected override Texture2D EffectImage { get { throw new NotImplementedException("REMEMBER TO IMPLEMENT DICK"); } }

        private TimeSpan _TickTimer = TimeSpan.FromSeconds(500); //so we get the first tick

        public override void Update(GameTime gameTime, AttributesComponent ac, Vector2 position)
        {
            base.Update(gameTime, ac, position);
            _TickTimer += gameTime.ElapsedGameTime;
            if (_TickTimer >= TimeSpan.FromSeconds(1)) //apply every second
            {
                float damage = ac.MaxHealth * Intensity;
                ac.CurrentHealth -= damage;
                _FloatingTexts.AddFloatingTexts(damage, Color.Purple);
                _TickTimer = TimeSpan.Zero;
            }
        }
    }

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
            if (_Timer >=  TimeSpan.FromSeconds(Duration))
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
