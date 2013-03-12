using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using CorvEngine;
using CorvEngine.Components;
using Microsoft.Xna.Framework;

namespace Corvus.Components
{
    class ProjectileComponent : Component
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
