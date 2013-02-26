using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace Corvus.Components
{
    /// <summary>
    /// A component used to cause a status effect when it collides with a Entity.
    /// </summary>
    public class CollisionStatusEffectComponent : CollisionEventComponent
    {
        private string _EffectType;
        private float _BaseValue;
        private float _Intensity;
        private float _Duration;

        /// <summary>
        /// The type of effect.
        /// </summary>
        public string EffectType
        {
            get { return _EffectType; }
            set { _EffectType = value; }
        }

        /// <summary>
        /// The static amount this effect will always apply.
        /// </summary>
        public float BaseValue
        {
            get { return _BaseValue; }
            set { _BaseValue = value; }
        }

        /// <summary>
        /// The intensity of the effect. Usually expressed as a percentage.
        /// </summary>
        public float Intensity
        {
            get { return _Intensity; }
            set { _Intensity = value; }
        }

        /// <summary>
        /// How long this effect should last in milliseconds.
        /// </summary>
        public float Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var se = Entity.GetComponent<StatusEffectsComponent>();
            if (se == null)
                return false;
            se.ApplyStatusEffect(EffectType, BaseValue, Intensity, Duration);
            return true;
        }
    }
}
