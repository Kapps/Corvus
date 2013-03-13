using System;
using System.Collections.Generic;
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
    /// A Component to set the status effect attributes. Mainly used for weapons and enemy attacks.
    /// </summary>
    public class StatusEffectPropertiesComponent : Component
    {
        private StatusEffectProperties _StatusEffectProperties = new StatusEffectProperties();

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public StatusEffectProperties StatusEffectAttributes { 
            get { return _StatusEffectProperties; }
            set { _StatusEffectProperties = value; }
        }

        /// <summary>
        /// The type of effect.
        /// </summary>
        public string EffectType
        {
            get { return StatusEffectAttributes.EffectType; }
            set { StatusEffectAttributes.EffectType = value; }
        }

        /// <summary>
        /// The static amount this effect will always apply.
        /// </summary>
        public float BaseValue
        {
            get { return StatusEffectAttributes.BaseValue; }
            set { StatusEffectAttributes.BaseValue = value; }
        }

        /// <summary>
        /// The intensity of the effect. Usually expressed as a percentage.
        /// </summary>
        public float Intensity
        {
            get { return StatusEffectAttributes.Intensity; }
            set { StatusEffectAttributes.Intensity = value; }
        }

        /// <summary>
        /// How long this effect should last in seconds.
        /// </summary>
        public float Duration
        {
            get { return StatusEffectAttributes.Duration; }
            set { StatusEffectAttributes.Duration = value; }
        }

    }
}
