using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;
using Microsoft.Xna.Framework;

namespace Corvus.Components
{
    /// <summary>
    /// A component used to handle the collision events for projectiles ONLY.
    /// </summary>
    public class CollisionProjectileComponent : CollisionEventComponent
    {
        private AttributesComponent AttributesComponent;
        private StatusEffectPropertiesComponent SEAComponent;
        private CombatPropertiesComponent CPComponent;
                
        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            bool colGood = false;
            //Apply normal damage.
            var dc = Entity.GetComponent<DamageComponent>();
            if (dc != null){
                dc.TakeDamage(AttributesComponent);
                colGood = true;
            }
            //Apply status effect if it can.
            if (CPComponent.AppliesEffect){
                var sc = Entity.GetComponent<StatusEffectsComponent>();
                if (sc != null){
                    sc.ApplyStatusEffect(SEAComponent.StatusEffectAttributes);
                    colGood = true;
                }
            }
            //Make an aoe appear if there should be one.
            if (CPComponent.IsAoE)
                AreaOfEffectComponent.CreateAoEEntity(this.Parent);

            return colGood;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            SEAComponent = Parent.GetComponent<StatusEffectPropertiesComponent>();
            CPComponent = this.GetDependency<CombatPropertiesComponent>();
        }
    }
}
