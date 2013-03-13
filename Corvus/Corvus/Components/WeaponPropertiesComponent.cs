using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay.Equipment;
using Microsoft.Xna.Framework;
using Corvus.Components.Gameplay;

namespace Corvus.Components
{
    /// <summary>
    /// A component to set the attributes for weapons.
    /// </summary>
    public class WeaponPropertiesComponent : Component
    {
        /// <summary>
        /// Gets the weapon data from this component.
        /// </summary>
        public WeaponProperties WeaponData { get { return _WeaponData; } }

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

        private WeaponProperties _WeaponData = new WeaponProperties();
    }
}
