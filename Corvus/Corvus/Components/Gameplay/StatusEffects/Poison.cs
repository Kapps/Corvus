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
    /// A status effect that causes damage over time. Damage is calculated by: (MaxHealth * intensity + BaseValue)
    /// </summary>
    public class Poison : StatusEffect
    {
        public override string Name { get { return "Poison"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Poison"; } }
        protected override bool IsGood { get { return false; } }
        
        protected override void OnFirstOccurance() {
            FloatingTextComponent.Add("Poison", Color.DarkViolet);
        }

        protected override void OnTick()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float damage = ac.MaxHealth * Attributes.Intensity + Attributes.BaseValue;
            ac.CurrentHealth -= damage;
            FloatingTextComponent.Add(damage, Color.DarkViolet);
        }

        protected override void OnFinished() { }

        //Unfortunately, need to specify this in order for StatusEffectsComponent to work properly.
        public Poison(Entity entity, StatusEffectProperties attributes) : base(entity, attributes) { }
    }
}
