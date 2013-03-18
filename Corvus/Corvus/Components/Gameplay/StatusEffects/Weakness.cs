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
    /// A status effect that reduces the strength of the affected entity. Weakness is calculated by: Intensity * Strength.
    /// </summary>
    public class Weakness : StatusEffect
    {
        public override string Name { get { return "Weakness"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Weakness"; } }
        protected override bool IsGood { get { return false; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Weakness", Color.Navy);
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.StrModifier *= Attributes.Intensity;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.StrModifier /= Attributes.Intensity;
        }

        public Weakness(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
