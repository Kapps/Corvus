using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.StatusEffects;

namespace Corvus.Components
{
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
        /// <param name="type">The nane of status effect.</param>
        /// <param name="baseValue">The base value of this effect.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">How long the effect should last.</param>
        public void ApplyStatusEffect(string name, float baseValue, float intensity, float duration)
        {
            Type type = Type.GetType(string.Format("Corvus.Components.Gameplay.StatusEffects.{0}", name));
            var statusEffect = Helper.GetObject<StatusEffect>(type);
            statusEffect.BaseValue = baseValue;
            statusEffect.Intensity = intensity;
            statusEffect.Duration = duration;
            statusEffect.EntitySize = this.Parent.Size;
            _StatusEffects.Add(statusEffect);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
        }
        //TODO: Find a better way to draw the effects.
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
                se.Update(Time, AttributesComponent.Attributes, ToScreen);
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

}
