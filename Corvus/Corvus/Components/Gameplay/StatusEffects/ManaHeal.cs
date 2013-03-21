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
    /// Heals the mana of the affected entity. Healing is calculated by: BaseValue and then every second, (MaxMana * Intensity)
    /// </summary>
    public class ManaHeal : StatusEffect
    {
        public override string Name { get { return "ManaHeal"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_ManaHeal"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Mana Heal", Color.DarkBlue);
            var ac = Entity.GetComponent<AttributesComponent>();
            float manaheal = Attributes.BaseValue;
            ac.CurrentMana += manaheal;
            FloatingTextComponent.Add(manaheal, Color.DarkBlue);
        }

        protected override void OnTick()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float manaHeal = ac.MaxMana * Attributes.Intensity;
            ac.CurrentMana += manaHeal;
            FloatingTextComponent.Add(manaHeal, Color.DarkBlue);
        }

        protected override void OnFinished() { }

        public ManaHeal(Entity entity, StatusEffectProperties sep) : base(entity, sep) { }
    }
}
