using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay.Equipment;

namespace Corvus.Components
{
    /// <summary>
    /// A collision component that, when a player collides with, will equip the specified weapon.
    /// </summary>
    public class CollisionEquipmentComponent : CollisionEventComponent
    {
        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var ec = Entity.GetComponent<EquipmentComponent>();
            if (ec == null)
                return false;
            var ac = this.GetDependency<AttributesComponent>();
            var wdc = this.GetDependency<WeaponPropertiesComponent>();
            var seac = Parent.GetComponent<StatusEffectAttributesComponent>();
            ec.EquipWeapon(new Weapon(wdc.WeaponData, ac.Attributes, seac.StatusEffectAttributes));
            return true;
        }
    }
}
