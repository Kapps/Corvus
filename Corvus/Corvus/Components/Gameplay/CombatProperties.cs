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
        private float _HitDelay = 0f;
        private bool _ConsumesMana = false;
        private float _AttackSlowDown = 1f;
        private string _ProjectileName = "";
        private Vector2 _ProjectileSize = new Vector2();
        private Vector2 _ProjectileOffset = new Vector2();
        private Vector2 _ProjectileVelocity = new Vector2();
        private float _ProjectileGravityCoefficient = 0f;
        private float _ProjectileHorDragCoefficient = 0f;
        private string _AoEName = "";
        private Vector2 _AoESize = new Vector2();
        private Vector2 _AoEOffset = new Vector2();
        private float _AoEDuration = 0f;
        private float _AoEDamagePercent = 0f;
        private EntityClassification _AoEHitableEntities = EntityClassification.Any;
        private bool _EnemyUseWeaponAnimation = false;
        private string _EnemyWeaponName = "";
        private Vector2 _EnemyWeaponOffset = new Vector2();
        
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
        /// Gets or sets whether this entity can attack with range.
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
        /// Gets or sets a percentage that indicates when the attack hit box should occur with respect to the attack speed.
        /// </summary>
        public float HitDelay
        {
            get { return _HitDelay; }
            set { _HitDelay = MathHelper.Clamp(value, 0f, 1f); }
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
        /// Gets or sets a percentage that indicates how much to slow down the movement speed of the entity while it is attacking.
        /// </summary>
        public float AttackSlowDown
        {
            get { return _AttackSlowDown; }
            set { _AttackSlowDown = MathHelper.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets or sets the projectile sprite to fire. 
        /// </summary>
        public string ProjectileName
        {
            get { return _ProjectileName; }
            set { _ProjectileName = value; }
        }

        /// <summary>
        /// Gets or sets the size of the projectile.
        /// </summary>
        public Vector2 ProjectileSize
        {
            get { return _ProjectileSize; }
            set { _ProjectileSize = value; }
        }

        /// <summary>
        /// Gets or sets how much to offset the projectile spawn point with respect to the center of the sprite.
        /// </summary>
        public Vector2 ProjectileOffset
        {
            get { return _ProjectileOffset; }
            set { _ProjectileOffset = value; }
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
        ///  Gets or sets the amount to multiply the force of gravity by.
        /// </summary>
        public float ProjectileGravityCoefficient
        {
            get { return _ProjectileGravityCoefficient; }
            set { _ProjectileGravityCoefficient = value; }
        }

        /// <summary>
        /// Gets or sets the amount to multiply the force of horizontal drag by.
        /// </summary>
        public float ProjectileHorDragCoefficient
        {
            get { return _ProjectileHorDragCoefficient; }
            set { _ProjectileHorDragCoefficient = value; }
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
        /// Gets or sets a value that indicates how much to offset the area of offset with respect to the center of the entity.
        /// </summary>
        public Vector2 AoEOffset
        {
            get { return _AoEOffset; }
            set { _AoEOffset = value; }
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

        /// <summary>
        /// Gets or sets the entities that are hitable by the area of effect.
        /// </summary>
        public EntityClassification AoEHitableEntities
        {
            get { return _AoEHitableEntities; }
            set { _AoEHitableEntities = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating the enemy should use a swinging weapon animation.
        /// </summary>
        public bool EnemyUseWeaponAnimation
        {
            get { return _EnemyUseWeaponAnimation; }
            set { _EnemyUseWeaponAnimation = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates which weapon to use as a swinging animation.
        /// </summary>
        public string EnemyWeaponName
        {
            get { return _EnemyWeaponName; }
            set { _EnemyWeaponName = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates how much to offset the weapon animation by.
        /// </summary>
        public Vector2 EnemyWeaponOffset
        {
            get { return _EnemyWeaponOffset; }
            set { _EnemyWeaponOffset = value; }
        }
    }
}
