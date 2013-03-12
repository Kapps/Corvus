using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;

namespace Corvus.Components
{
    /// <summary>
    /// A component used to handle the collision events for projectiles ONLY.
    /// </summary>
    public class CollisionProjectileComponent : CollisionEventComponent
    {
        /// <summary>
        /// Gets or sets a value indicating this projectile also applies a status effect. SHOULD ONLY BE SET IN CODE.
        /// </summary>
        public bool AppliesEffect
        {
            get { return _AppliesEffect; }
            set { _AppliesEffect = value; }
        }

        private bool _AppliesEffect = false;
        private AttributesComponent AttributesComponent;
        private StatusEffectAttributesComponent SEAComponent;

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
            if (AppliesEffect)
            {
                var sc = Entity.GetComponent<StatusEffectsComponent>();
                if (sc != null){
                    sc.ApplyStatusEffect(SEAComponent.StatusEffectAttributes);
                    colGood = true;
                }
            }

            return colGood;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            SEAComponent = Parent.GetComponent<StatusEffectAttributesComponent>();
        }
    }
}
