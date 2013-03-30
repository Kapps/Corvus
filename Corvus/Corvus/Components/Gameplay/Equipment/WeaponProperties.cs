using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Corvus.Components.Gameplay;

namespace Corvus.Components.Gameplay.Equipment
{
    /// <summary>
    /// A class to manage all the weapon details.
    /// </summary>
    public class WeaponProperties
    {
        /// <summary>
        /// Gets or sets the weapon name.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Gets or sets the type of this weapon. This determines the animation to play.
        /// </summary>
        public WeaponTypes WeaponType
        {
            get { return _WeaponType; }
            set { _WeaponType = value; }
        }

        /// <summary>
        /// Gets the animation name.
        /// </summary>
        public string AnimationName { 
            get {
                string type = (WeaponType == WeaponTypes.Spell || WeaponType == WeaponTypes.Bow) ? WeaponType.ToString() : "Melee";
                return type + "Attack";
            }
        }

        /// <summary>
        /// Gets a value indicating that his weapon is melee.
        /// </summary>
        public bool IsMelee { get { return !(WeaponType == WeaponTypes.Spell || WeaponType == WeaponTypes.Bow); } }

        /// <summary>
        /// Gets the name of this weapon based on it's entity name. HACK, i know.
        /// </summary>
        public string SystemName { get { return Name.Replace(" ", ""); } }

        private string _Name = "";
        private WeaponTypes _WeaponType;
    }
}
