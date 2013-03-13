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

        /// <summary>
        /// Gets or sets a value that determines if this weapon is a ranged one.
        /// </summary>
        public bool IsRanged
        {
            get { return WeaponData.IsRanged; }
            set { WeaponData.IsRanged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this weapon applies a status effect.
        /// </summary>
        public bool AppliesEffect
        {
            get { return WeaponData.AppliesEffect; }
            set { WeaponData.AppliesEffect = value; }
        }

        /// <summary>
        /// Gets or sets the projectile to fire. 
        /// </summary>
        public string ProjectileName
        {
            get { return WeaponData.ProjectileName; }
            set { WeaponData.ProjectileName = value; }
        }

        /// <summary>
        /// Gets or sets the projectile velocity.
        /// </summary>
        public Vector2 ProjectileVelocity
        {
            get { return WeaponData.ProjectileVelocity; }
            set { WeaponData.ProjectileVelocity = value; }
        }

        private WeaponProperties _WeaponData = new WeaponProperties();
    }
}
