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
    /// I WORK OUT. 
    /// </summary>
    public class IWorkOut: StatusEffect
    {
        public override string Name { get { return "IWorkOut"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_IWorkOut"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("I Work Out!!!", Color.Crimson);

            var ac = Entity.GetComponent<AttributesComponent>();
            ac.StrModifier *= 1 + Attributes.Intensity;
            ac.DexModifier *= 1 + Attributes.Intensity;
            ac.KnockbackModifier *= 5f;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            ac.StrModifier /= 1 + Attributes.Intensity;
            ac.DexModifier /= 1 + Attributes.Intensity;
            ac.KnockbackModifier /= 5f;
        }

        public IWorkOut(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
