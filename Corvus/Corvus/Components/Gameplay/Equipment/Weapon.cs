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
        public WeaponProperties WeaponData { get; set; }

        /// <summary>
        /// Gets or sets the weapon combat properties. 
        /// </summary>
        public CombatProperties CombatProperties { get; set; }

        /// <summary>
        /// Gets or sets the attributes for this weapon.
        /// </summary>
        public Attributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets the status effect for this weapon, if there is one.
        /// </summary>
        public StatusEffectProperties Effect { get; set; }

        /// <summary>
        /// Creates a new weapon with specified name and attributes.
        /// </summary>
        public Weapon(WeaponProperties data, CombatProperties propes, Attributes attri, StatusEffectProperties effect)
        {
            WeaponData = data;
            CombatProperties = propes;
            Attributes = attri;
            Effect = effect;
        }
    }
}
