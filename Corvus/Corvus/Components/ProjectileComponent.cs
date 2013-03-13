using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using CorvEngine;
using CorvEngine.Components;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;
using Corvus.Components.Gameplay.StatusEffects;
using Microsoft.Xna.Framework;

namespace Corvus.Components
{
    public class ProjectileComponent : Component
    {
        protected override void OnUpdate(GameTime Time)
        {
            var clc = Parent.GetComponent<ClassificationComponent>();
            var pc = Parent.GetComponent<PhysicsComponent>();
            
            if (clc.Classification == EntityClassification.Projectile && pc.IsGrounded)
                Parent.Dispose();

            base.OnUpdate(Time);
        }
    }
}
