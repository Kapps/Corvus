using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corvus.Components.Gameplay.Equipment
{
    public class WeaponData
    {
        /// <summary>
        /// Gets or sets the weapon name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of this weapon. This determines the animation to play.
        /// </summary>
        public WeaponTypes WeaponType { get; set; }

        /// <summary>
        /// Gets or sets whether this weapon is a ranged weapon or not.
        /// </summary>
        public bool IsRanged{ get; set; }
        
        /// <summary>
        /// Gets the animation name.
        /// </summary>
        public string AnimationName { get { return WeaponType.ToString() + "Attack"; } }
    }
}
