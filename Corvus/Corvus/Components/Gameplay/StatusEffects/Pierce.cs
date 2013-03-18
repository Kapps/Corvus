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
    /// A status effect that reduces the dexterity of the affected entity. Pierce is calculated by: Intensity * Dexterity.
    /// </summary>
    public class Pierce : StatusEffect
    {
        public override string Name { get { return "Pierce"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Pierce"; } }
        protected override bool IsGood { get { return false; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Pierce", Color.Navy);
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.DexModifier *= Attributes.Intensity;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.DexModifier -= Attributes.Intensity;
        }

        public Pierce(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
