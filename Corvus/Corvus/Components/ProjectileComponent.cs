using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using CorvEngine;
using CorvEngine.Components;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;
using Corvus.Components.Gameplay.StatusEffects;
using Microsoft.Xna.Framework;

namespace Corvus.Components
{
    /// <summary>
    /// A component to manage a projectile. This should only be used by Projectile.txt.
    /// </summary>
    public class ProjectileComponent : CollisionEventComponent
    {
        private AttributesComponent AttributesComponent;
        private StatusEffectPropertiesComponent SEAComponent;
        private CombatPropertiesComponent CPComponent;
        private PhysicsComponent PC;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            SEAComponent = Parent.GetComponent<StatusEffectPropertiesComponent>();
            CPComponent = this.GetDependency<CombatPropertiesComponent>();
            PC = this.GetDependency<PhysicsComponent>();
        }

        protected override void OnUpdate(GameTime Time)
        {
            if (PC.IsGrounded || PC.VelocityX == 0f)
            {
                if(CPComponent.IsAoE)
                    AreaOfEffectComponent.CreateAoEEntity(this.Parent);
                if (!string.IsNullOrEmpty(CPComponent.ProjectileOnHitSound))
                    AudioManager.PlaySoundEffect(CPComponent.ProjectileOnHitSound);
                Parent.Dispose();
            }
            base.OnUpdate(Time);
        }

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            bool colGood = false;
            //Apply normal damage.
            var dc = Entity.GetComponent<DamageComponent>();
            if (dc != null)
            {
                dc.TakeDamage(AttributesComponent);
                colGood = true;
            }

            var mc = Entity.GetComponent<MovementComponent>();
            if (mc != null)
            {
                mc.Knockback(AttributesComponent.TotalKnockback, (PC.VelocityX > 0) ? 1 : -1);
                colGood = true;
            }

            //Apply status effect if it can.
            if (CPComponent.AppliesEffect)
            {
                var sc = Entity.GetComponent<StatusEffectsComponent>();
                if (sc != null)
                {
                    sc.ApplyStatusEffect(SEAComponent.StatusEffectAttributes);
                    colGood = true;
                }
            }
            //Make an aoe appear if there should be one.
            if (CPComponent.IsAoE) {
                AreaOfEffectComponent.CreateAoEEntity(this.Parent);
                colGood = true;
            }

            if (!string.IsNullOrEmpty(CPComponent.ProjectileOnHitSound))
                AudioManager.PlaySoundEffect(CPComponent.ProjectileOnHitSound);

            return colGood;
        }

        //NOTE: Not tested with enemies yet :p
        /// <summary>
        /// Creates a projectile entity next to the entity calling this function.
        /// The launch direction can be set manually to avoid awkward projectiles, otherwise it uses the movement components current direction.
        /// </summary>
        public static void CreateProjectileEntity(Entity entity, Direction? launchDirection = null)
        {
            var CPComponent = entity.GetComponent<CombatPropertiesComponent>();
            var AttributesComponent = entity.GetComponent<AttributesComponent>();
            var EquipmentComponent = entity.GetComponent<EquipmentComponent>();
            var CombatComponent = entity.GetComponent<CombatComponent>();
            var MovementComponent = entity.GetComponent<MovementComponent>();
            var SEAComponent = entity.GetComponent<StatusEffectPropertiesComponent>();

            var projectile = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("Projectile").CreateEntity();
            projectile.Size = new Vector2(CPComponent.ProjectileSize.X, CPComponent.ProjectileSize.Y);
            var center = entity.Location.Center;
            projectile.Position = new Vector2(center.X + CorvusExtensions.GetSign(MovementComponent.CurrentDirection) * (CPComponent.ProjectileOffset.X),
                                                center.Y + CPComponent.ProjectileOffset.Y);
            
            entity.Scene.AddEntity(projectile);
            string spriteName = CPComponent.ProjectileName;
            var sprite = CorvusGame.Instance.GlobalContent.LoadSprite(spriteName);
            var sc = projectile.GetComponent<SpriteComponent>();
            sc.Sprite = sprite;

            //Apply properties
            var ac = projectile.GetComponent<AttributesComponent>();
            ac.Attributes = AttributesComponent.Attributes;
            if (CPComponent.AppliesEffect)
            {
                var seac = projectile.GetComponent<StatusEffectPropertiesComponent>();
                seac.StatusEffectAttributes = (SEAComponent != null) ? SEAComponent.StatusEffectAttributes : EquipmentComponent.CurrentWeapon.Effect;
            }
            if(EquipmentComponent.UseWeaponBonuses)// != null)
            {
                var ec = projectile.GetComponent<EquipmentComponent>();
                ec.UseWeaponBonuses = true;
                ec.EquipWeapon(EquipmentComponent.CurrentWeapon);
            }
            var cpc = projectile.GetComponent<CombatPropertiesComponent>();
            cpc.CombatProperties = CPComponent.CombatProperties;
            var pc = projectile.GetComponent<ProjectileComponent>();
            pc.Classification = CombatComponent.AttackableEntities;
            var physC = projectile.GetComponent<PhysicsComponent>();
            physC.GravityCoefficient = CPComponent.ProjectileGravityCoefficient;
            physC.HorizontalDragCoefficient = CPComponent.ProjectileHorDragCoefficient;
            var direction = (launchDirection == null) ? MovementComponent.CurrentDirection : launchDirection;
            if (direction == Direction.Right)
                physC.Velocity = new Vector2(CPComponent.CombatProperties.ProjectileVelocity.X, -CPComponent.CombatProperties.ProjectileVelocity.Y);
            else if (direction == Direction.Left)
                physC.Velocity = new Vector2(-CPComponent.CombatProperties.ProjectileVelocity.X, -CPComponent.CombatProperties.ProjectileVelocity.Y);
            var mc = projectile.GetComponent<MovementComponent>();
            mc.CurrentDirection = MovementComponent.CurrentDirection;
        }
    }
}
