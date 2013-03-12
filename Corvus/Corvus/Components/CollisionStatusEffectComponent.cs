using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace Corvus.Components
{
    /// <summary>
    /// A component used to cause a status effect when it collides with a Entity.
    /// </summary>
    public class CollisionStatusEffectComponent : CollisionEventComponent
    {
        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var se = Entity.GetComponent<StatusEffectsComponent>();
            if (se == null)
                return false;
            var seac = this.GetDependency<StatusEffectAttributesComponent>();
            se.ApplyStatusEffect(seac.StatusEffectAttributes);
            return true;
        }
    }
}
