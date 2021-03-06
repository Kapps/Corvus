﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corvus.Components.Gameplay.StatusEffects
{
    /// <summary>
    /// A class that contains the properties for a status effect.
    /// </summary>
    public class StatusEffectProperties
    {
        /// <summary>
        /// Gets or sets the name of this status effect.
        /// </summary>
        public string EffectType
        {
            get { return _EffectType; }
            set { _EffectType = value; }
        }

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

        public string _EffectType = "";
        private float _BaseValue = 0;
        private float _Intensity = 1f;
        private float _Duration = 0;

    }
}
