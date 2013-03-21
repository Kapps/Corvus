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
    /// A status effect that increases the movement speed of the affected entity. Fleet is based on: Intensity * (Walk, attack)Speed.
    /// </summary>
    public class Fleet  : StatusEffect
    {
        public override string Name { get { return "Fleet"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Fleet"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Fleet", Color.Crimson);
            var mc = Entity.GetComponent<MovementComponent>();
            mc.WalkSpeedModifier *= 1 + Attributes.Intensity;

            var ac = Entity.GetComponent<AttributesComponent>();
            ac.AttackSpeedModifier *= Attributes.Intensity;
        }

        protected override void OnTick() { }

        protected override void OnFinished()
        {
            var mc = Entity.GetComponent<MovementComponent>();
            mc.WalkSpeedModifier /= 1 + Attributes.Intensity;

            var ac = Entity.GetComponent<AttributesComponent>();
            ac.AttackSpeedModifier /= Attributes.Intensity;
        }

        public Fleet(Entity entity, StatusEffectProperties prop) : base(entity, prop) { }
    }
}
