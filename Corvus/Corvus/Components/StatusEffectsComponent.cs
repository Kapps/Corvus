﻿using System;
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
        /// Applies a status effect to this entity
        /// </summary>
        public void ApplyStatusEffect(StatusEffectProperties attributes)
        {
            var constructor = Helper.GetObjectConstructor<StatusEffect>(string.Format("Corvus.Components.Gameplay.StatusEffects.{0}", attributes.EffectType), new Type[] { typeof(Entity), typeof(StatusEffectProperties) });
            var statusEffect = constructor(this.Parent, attributes);
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
            foreach (StatusEffect se in _StatusEffects.Reverse<StatusEffect>())
            {
                se.Update(Time);
                if (se.IsFinished)
                    _StatusEffects.Remove(se);
            }
        }
        //TODO: The offsets are hardcoded.
        protected override void OnDraw()
        {
            base.OnDraw();
            var entityPos = Camera.Active.WorldToScreen(this.Parent.Location);
            var position = new Vector2(entityPos.X, entityPos.Bottom + 5);
            float xOffset = 0f;
            foreach (StatusEffect se in _StatusEffects)
            {
                se.Draw(position);
                xOffset += 17f;
                position.X += xOffset;
            }
        }

    }

}
