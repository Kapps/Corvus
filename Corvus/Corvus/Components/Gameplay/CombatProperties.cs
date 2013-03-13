using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;
using Microsoft.Xna.Framework;

namespace Corvus.Components.Gameplay
{
    /// <summary>
    /// A class to tract all combat properties.
    /// </summary>
    public class CombatProperties 
    {
        private bool _IsMelee = true;
        private bool _IsRanged = false;
        private bool _AppliesEffect = false;
        private bool _IsAoE = false;
        private bool _ConsumesMana = false;
        private float _ManaCost = 0f;
        private string _ProjectileName = "";
        private Vector2 _ProjectileVelocity = new Vector2();
        private string _AoEName = "";
        private Vector2 _AoESize = new Vector2();
        private float _AoEDuration = 0f;
        private float _AoEDamagePercent = 0f;

        /// <summary>
        /// Gets or sets whether this entity can attack with melee.
        /// Enemy Only.
        /// </summary>
        public bool IsMelee
        {
            get { return _IsMelee; }
            set { _IsMelee = value; }
        }

        /// <summary>
        /// Gets or sets whether this entity can attack with range
        /// </summary>
        public bool IsRanged
        {
            get { return _IsRanged; }
            set { _IsRanged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this entity applies a status effect.
        /// </summary>
        public bool AppliesEffect
        {
            get { return _AppliesEffect; }
            set { _AppliesEffect = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this entity has an area of effect attack.
        /// </summary>
        public bool IsAoE
        {
            get { return _IsAoE; }
            set { _IsAoE = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entity consumes mana.
        /// Player Only.
        /// </summary>
        public bool ConsumesMana
        {
            get { return _ConsumesMana; }
            set { _ConsumesMana = value; }
        }

        /// <summary>
        /// How much mana to consume.
        /// Player Only.
        /// </summary>
        public float ManaCost
        {
            get { return _ManaCost; }
            set { _ManaCost = value; }
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
        /// Gets or sets the name of the sprite to draw for this area of effect.
        /// </summary>
        public string AoEName
        {
            get { return _AoEName; }
            set { _AoEName = value; }
        }

        /// <summary>
        /// Gets or sets the size of the AoE.
        /// </summary>
        public Vector2 AoESize
        {
            get { return _AoESize; }
            set { _AoESize = value; }
        }
        /// <summary>
        /// Gets or sets the duration the area of effect should linger.
        /// </summary>
        public float AoEDuration
        {
            get { return _AoEDuration; }
            set { _AoEDuration = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets how much to reduce the damage for the area of effect.
        /// </summary>
        public float AoEDamagePercent
        {
            get { return _AoEDamagePercent; }
            set { _AoEDamagePercent = MathHelper.Clamp(value, 0f, 1f); }
        }


    }
}
