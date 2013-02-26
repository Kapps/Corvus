using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace Corvus.Components
{
    /// <summary>
    /// A collision component that, when a player collides with, will equip the specified weapon.
    /// </summary>
    public class CollisionEquipmentComponent : CollisionEventComponent
    {
        /// <summary>
        /// Gets or sets the name of the weapon type.
        /// </summary>
        public string WeaponType
        {
            get { return _WeaponType; }
            set { _WeaponType = value; }
        }

        private string _WeaponType;

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var ec = Entity.GetComponent<EquipmentComponent>();
            if (ec == null)
                return false;
            var ac = this.GetDependency<AttributesComponent>();
            ec.EquipWeapon(WeaponType, ac);
            return true;
        }
    }
}
