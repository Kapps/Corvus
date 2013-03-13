using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Corvus.Components.Gameplay.Equipment
{
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
        /// Gets or sets whether this weapon is a ranged weapon or not.
        /// </summary>
        public bool IsRanged
        {
            get { return _IsRanged; }
            set { _IsRanged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this weapon applies a status effect.
        /// </summary>
        public bool AppliesEffect
        {
            get { return _AppliesEffect; }
            set { _AppliesEffect = value; }
        }

        /// <summary>
        /// Gets or sets the projectile to fire. 
        /// </summary>
        public string ProjectileName
        {
            get { return _ProjectileName; }
            set { _ProjectileName = value; }
        }

        /// <summary>
        /// Gets or sets the projectile velocity.
        /// </summary>
        public Vector2 ProjectileVelocity
        {
            get { return _ProjectileVelocity; }
            set { _ProjectileVelocity = value; }
        }

        /// <summary>
        /// Gets the animation name.
        /// </summary>
        public string AnimationName { get { return WeaponType.ToString() + "Attack"; } }

        private string _Name = "";
        private WeaponTypes _WeaponType;
        private bool _IsRanged = false;
        private bool _AppliesEffect = false;
        private string _ProjectileName = "";
        private Vector2 _ProjectileVelocity = new Vector2();
    }
}
