using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay.Equipment;
using Microsoft.Xna.Framework;

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
            var cpc = this.GetDependency<CombatPropertiesComponent>();
            var wdc = this.GetDependency<WeaponPropertiesComponent>();
            var seac = Parent.GetComponent<StatusEffectPropertiesComponent>();
            ec.EquipWeapon(new Weapon(wdc.WeaponData, cpc.CombatProperties, ac.Attributes, seac.StatusEffectAttributes));

            var ftc = Entity.GetComponent<FloatingTextComponent>();
            ftc.Add(wdc.WeaponData.Name, Color.Black);

            return true;
        }
    }
}
