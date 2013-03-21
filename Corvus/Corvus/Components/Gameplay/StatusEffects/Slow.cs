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
    /// A status effect that reduces the movement speed and jump speed of the affected entity. Slow is calculated by: Intensity * (walk, attack, jump)Speed
    /// </summary>
    public class Slow : StatusEffect
    {
        public override string Name { get { return "Slow"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Slow"; } }
        protected override bool IsGood { get { return false; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Slow", Color.Navy);
            var mc = Entity.GetComponent<MovementComponent>();
            mc.WalkSpeedModifier *= Attributes.Intensity;
            mc.JumpSpeedModifier *= Attributes.Intensity;

            var ac = Entity.GetComponent<AttributesComponent>();
            ac.AttackSpeedModifier *= 1 + Attributes.Intensity;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var mc = Entity.GetComponent<MovementComponent>();
            mc.WalkSpeedModifier /= Attributes.Intensity;
            mc.JumpSpeedModifier /= Attributes.Intensity;

            var ac = Entity.GetComponent<AttributesComponent>();
            ac.AttackSpeedModifier /= 1 + Attributes.Intensity;
        }

        public Slow(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
