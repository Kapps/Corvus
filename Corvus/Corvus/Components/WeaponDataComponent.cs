using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay.Equipment;

namespace Corvus.Components
{
    /// <summary>
    /// A component to set the attributes for weapons.
    /// </summary>
    public class WeaponDataComponent : Component
    {
        /// <summary>
        /// Gets the weapon data from this component.
        /// </summary>
        public WeaponData WeaponData { get { return _WeaponData; } }

        /// <summary>
        /// Gets or sets the weapon name.
        /// </summary>
        public string WeaponName 
        {
            get { return WeaponData.Name; }
            set { WeaponData.Name = value; }
        }

        /// <summary>
        /// Gets or sets the weapon type.
        /// </summary>
        public WeaponTypes WeaponType
        {
            get { return WeaponData.WeaponType; }
            set { WeaponData.WeaponType= value; }
        }

        /// <summary>
        /// Gets or sets a value that determines if this weapon is a ranged one.
        /// </summary>
        public bool IsRanged
        {
            get { return WeaponData.IsRanged; }
            set { WeaponData.IsRanged = value; }
        }

        private WeaponData _WeaponData = new WeaponData();
    }
}
