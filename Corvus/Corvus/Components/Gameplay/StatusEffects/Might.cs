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
    /// <summary>
    /// A status effect that increases the strength of the affected entity. Might is determined by: Intensity * Strength.
    /// </summary>
    public class Might : StatusEffect
    {
        public override string Name { get { return "Might"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Might"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Might", Color.Crimson);
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.StrModifier *= 1 + Attributes.Intensity;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.StrModifier /= 1 + Attributes.Intensity;
        }

        public Might(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
