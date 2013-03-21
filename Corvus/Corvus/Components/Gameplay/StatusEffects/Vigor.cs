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
    /// A status effect that increases the dexterity of the affected entity. Vigor is based on: Intensity * Dexterity.
    /// </summary>
    public class Vigor: StatusEffect
    {
        public override string Name { get { return "Vigor"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Vigor"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Vigor", Color.Crimson);
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.DexModifier *= 1 + Attributes.Intensity;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.DexModifier /= 1 + Attributes.Intensity;
        }

        public Vigor(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
