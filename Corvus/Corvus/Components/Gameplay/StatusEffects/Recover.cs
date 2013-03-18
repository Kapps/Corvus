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
    /// A status effect that heals both health and mana. Recover is calculated by: Basevalue and then, Intensity * Max(Health,Mana)
    /// </summary>
    public class Recover : StatusEffect
    {
        public override string Name { get { return "Recover"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Recover"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Recover", Color.LightCyan);
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.CurrentHealth += Attributes.BaseValue;
            ac.CurrentMana += Attributes.BaseValue;
        }

        protected override void OnTick() 
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float heal = ac.MaxHealth * Attributes.Intensity;
            ac.CurrentHealth += heal;
            FloatingTextComponent.Add(heal, Color.Aqua);
            float mana = ac.MaxMana * Attributes.Intensity;
            ac.CurrentMana += mana;
            FloatingTextComponent.Add(mana, Color.DarkBlue);
        }

        protected override void OnFinished() { }

        public Recover(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
