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
        public string AnimationName { get { return WeaponType.ToString() + "Attack"; } }

        private string _Name = "";
        private WeaponTypes _WeaponType;

    }
}
