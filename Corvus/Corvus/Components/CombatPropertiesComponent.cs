using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;
using Microsoft.Xna.Framework;

namespace Corvus.Components
{
    /// <summary>
    /// A component to handle all combat components. For players, these are set through weapons, otherwise, this is the place to do it.
    /// </summary>
    public class CombatPropertiesComponent : Component
    {
        /// <summary>
        /// Gets the combat properties for this component.
        /// </summary>
        public CombatProperties CombatProperties
        {
            get { 
                //The first check is there to avoid a bug when creating the blueprint.
                if(EquipmentComponent == null || !EquipmentComponent.UseWeaponBonuses)
                    return _CombatProperties;
                return EquipmentComponent.CurrentWeapon.CombatProperties;
            }
            set { _CombatProperties = value; }
        }

        /// <summary>
        /// Gets or sets whether this entity can attack with melee.
        /// Enemy Only.
        /// </summary>
        public bool IsMelee
        {
            get { return CombatProperties.IsMelee; }
            set { CombatProperties.IsMelee = value; }
        }

        /// <summary>
        /// Gets or sets whether this entity can attack with range
        /// </summary>
        public bool IsRanged
        {
            get { return CombatProperties.IsRanged; }
            set { CombatProperties.IsRanged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this entity applies a status effect.
        /// </summary>
        public bool AppliesEffect
        {
            get { return CombatProperties.AppliesEffect; }
            set { CombatProperties.AppliesEffect = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this entity has an area of effect attack.
        /// </summary>
        public bool IsAoE
        {
            get { return CombatProperties.IsAoE; }
            set { CombatProperties.IsAoE = value; }
        }

        /// <summary>
        /// Gets or sets a percentage that indicates when the attack hit box should occur with respect to the attack speed.
        /// </summary>
        public float HitDelay
        {
            get { return CombatProperties.HitDelay; }
            set { CombatProperties.HitDelay = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entity consumes mana.
        /// Player Only.
        /// </summary>
        public bool ConsumesMana
        {
            get { return CombatProperties.ConsumesMana; }
            set { CombatProperties.ConsumesMana = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates how much to slow down the movement speed of the entity while it is attacking.
        /// </summary>
        public float AttackSlowDown
        {
            get { return CombatProperties.AttackSlowDown; }
            set { CombatProperties.AttackSlowDown = value; }
        }

        /// <summary>
        /// Gets or sets the projectile sprite to fire. 
        /// </summary>
        public string ProjectileName
        {
            get { return CombatProperties.ProjectileName; }
            set { CombatProperties.ProjectileName = value; }
        }

        /// <summary>
        /// Gets or sets the size of the projectile.
        /// </summary>
        public Vector2 ProjectileSize
        {
            get { return CombatProperties.ProjectileSize; }
            set { CombatProperties.ProjectileSize = value; }
        }

        /// <summary>
        /// Gets or sets how much to offset the projectile spawn point with respect to the center of the sprite.
        /// </summary>
        public Vector2 ProjectileOffset
        {
            get { return CombatProperties.ProjectileOffset; }
            set { CombatProperties.ProjectileOffset = value; }
        }

        /// <summary>
        /// Gets or sets the projectile velocity.
        /// </summary>
        public Vector2 ProjectileVelocity
        {
            get { return CombatProperties.ProjectileVelocity; }
            set { CombatProperties.ProjectileVelocity = value; }
        }

        /// <summary>
        ///  Gets or sets the amount to multiply the force of gravity by.
        /// </summary>
        public float ProjectileGravityCoefficient
        {
            get { return CombatProperties.ProjectileGravityCoefficient; }
            set { CombatProperties.ProjectileGravityCoefficient = value; }
        }

        /// <summary>
        /// Gets or sets the amount to multiply the force of horizontal drag by.
        /// </summary>
        public float ProjectileHorDragCoefficient
        {
            get { return CombatProperties.ProjectileHorDragCoefficient; }
            set { CombatProperties.ProjectileHorDragCoefficient = value; }
        }

        /// <summary>
        /// Gets or sets the name of the entity to draw for this area of effect.
        /// </summary>
        public string AoEName
        {
            get { return CombatProperties.AoEName; }
            set { CombatProperties.AoEName = value; }
        }

        /// <summary>
        /// Gets or sets the size of the AoE.
        /// </summary>
        public Vector2 AoESize
        {
            get { return CombatProperties.AoESize; }
            set { CombatProperties.AoESize = value; }
        }

        /// <summary>
        /// Gets or sets the duration the area of effect should linger.
        /// </summary>
        public float AoEDuration
        {
            get { return CombatProperties.AoEDuration; }
            set { CombatProperties.AoEDuration = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets how much to reduce the damage for the area of effect.
        /// </summary>
        public float AoEDamagePercent
        {
            get { return CombatProperties.AoEDamagePercent; }
            set { CombatProperties.AoEDamagePercent = MathHelper.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets or sets the entities that are hitable by the area of effect.
        /// </summary>
        public EntityClassification AoEHitableEntities
        {
            get { return CombatProperties.AoEHitableEntities; }
            set { CombatProperties.AoEHitableEntities = value; }
        }

        private CombatProperties _CombatProperties = new CombatProperties();
        private EquipmentComponent EquipmentComponent;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            EquipmentComponent = Parent.GetComponent<EquipmentComponent>();
        }
    }
}
