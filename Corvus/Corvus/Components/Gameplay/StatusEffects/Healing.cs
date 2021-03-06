﻿using System;
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
    /// Heals the health of the affected entity. Healing is calculated by: BaseValue and then every second, (MaxHealth * Intensity)
    /// </summary>
    public class Healing : StatusEffect
    {
        public override string Name { get { return "Healing"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/Effect_Healing"; } }
        protected override bool IsGood { get { return true; } }

        protected override void OnFirstOccurance()
        {
            FloatingTextComponent.Add("Healing", Color.Aqua);
            var ac = Entity.GetComponent<AttributesComponent>();
            float heal = Attributes.BaseValue;
            ac.CurrentHealth += heal;
            FloatingTextComponent.Add(heal, Color.Aqua);
        }

        protected override void OnTick()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float heal = ac.MaxHealth * Attributes.Intensity;
            ac.CurrentHealth += heal;
            FloatingTextComponent.Add(heal, Color.Aqua);
        }

        protected override void OnFinished() { }

        public Healing(Entity entity, StatusEffectProperties attributes) : base(entity, attributes) { }
    }
}
