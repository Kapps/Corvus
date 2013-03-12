using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay.StatusEffects;

namespace Corvus.Components.Gameplay.Equipment
{
    /// <summary>
    /// The base class for a weapon.
    /// </summary>
    public class Weapon
    {
        /// <summary>
        /// Gets or sets the weapon data. Should be the same name as the file.
        /// </summary>
        public WeaponData WeaponData { get; set; }

        /// <summary>
        /// The attributes from this weapon.
        /// </summary>
        public Attributes Attributes { get; set; }
        
        /// <summary>
        /// Creates a new weapon with specified name and attributes.
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="attri"></param>
        public Weapon(WeaponData data, Attributes attri)
        {
            WeaponData = data;
            Attributes = attri;
        }
    }
}
