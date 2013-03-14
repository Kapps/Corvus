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
    public class ProjectileComponent : CollisionEventComponent
    {
        private AttributesComponent AttributesComponent;
        private StatusEffectPropertiesComponent SEAComponent;
        private CombatPropertiesComponent CPComponent;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            SEAComponent = Parent.GetComponent<StatusEffectPropertiesComponent>();
            CPComponent = this.GetDependency<CombatPropertiesComponent>();
        }

        protected override void OnUpdate(GameTime Time)
        {
            var clc = Parent.GetComponent<ClassificationComponent>();
            var pc = Parent.GetComponent<PhysicsComponent>();

            if (clc.Classification == EntityClassification.Projectile && pc.IsGrounded)
                Parent.Dispose();

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
            if (CPComponent.IsAoE)
                AreaOfEffectComponent.CreateAoEEntity(this.Parent);

            return colGood;
        }

        //NOTE: Not tested with enemies yet :p
        /// <summary>
        /// Creates a projectile entity by the entity calling this function. 
        /// </summary>
        public static void CreateProjectileEntity(Entity entity)
        {
            var CPComponent = entity.GetComponent<CombatPropertiesComponent>();
            var SEAComponent = entity.GetComponent<StatusEffectPropertiesComponent>();
            var AttributesComponent = entity.GetComponent<AttributesComponent>();
            var EquipmentComponent = entity.GetComponent<EquipmentComponent>();
            var CombatComponent = entity.GetComponent<CombatComponent>();
            var MovementComponent = entity.GetComponent<MovementComponent>();

            var projectile = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("Projectile").CreateEntity();
            if (EquipmentComponent == null)
            {
                var aoeEC = projectile.GetComponent<EquipmentComponent>();
                projectile.Components.Remove(aoeEC);
            }
            projectile.Size = (EquipmentComponent == null) ? new Vector2(CPComponent.ProjectileSize.X, CPComponent.ProjectileSize.Y) :
                                                             new Vector2(EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileSize.X, EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileSize.Y);

            projectile.Position = new Vector2(entity.Location.Center.X, entity.Location.Top);
            entity.Scene.AddEntity(projectile);

            string spriteName = (EquipmentComponent == null) ? CPComponent.ProjectileName : EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileName;
            var sprite = CorvusGame.Instance.GlobalContent.LoadSprite(spriteName);
            var sc = projectile.GetComponent<SpriteComponent>();
            sc.Sprite = sprite;

            //A bit of a hack to get the projectile to apply damage and status effects.
            var ac = projectile.GetComponent<AttributesComponent>();
            ac.Attributes = AttributesComponent.Attributes;
            if (EquipmentComponent != null)
            {
                var ec = projectile.GetComponent<EquipmentComponent>();
                ec.EquipWeapon(EquipmentComponent.CurrentWeapon);
                if (EquipmentComponent.CurrentWeapon.CombatProperties.AppliesEffect)
                {
                    var seac = projectile.GetComponent<StatusEffectPropertiesComponent>();
                    seac.StatusEffectAttributes = EquipmentComponent.CurrentWeapon.Effect;
                }
            }
            else
            {
                if (CPComponent.AppliesEffect)
                {
                    var seac = projectile.GetComponent<StatusEffectPropertiesComponent>();
                    seac.StatusEffectAttributes = SEAComponent.StatusEffectAttributes;
                }
            }

            var cpc = projectile.GetComponent<CombatPropertiesComponent>();
            cpc.CombatProperties = (EquipmentComponent == null) ? CPComponent.CombatProperties : EquipmentComponent.CurrentWeapon.CombatProperties;

            var pc = projectile.GetComponent<ProjectileComponent>();
            pc.Classification = CombatComponent.AttackableEntities;

            var physC = projectile.GetComponent<PhysicsComponent>();
            if (MovementComponent.CurrentDirection == Direction.Right)
                physC.Velocity = new Vector2(EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileVelocity.X, -EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileVelocity.Y);
            else if (MovementComponent.CurrentDirection == Direction.Left)
                physC.Velocity = new Vector2(-EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileVelocity.X, -EquipmentComponent.CurrentWeapon.CombatProperties.ProjectileVelocity.Y);

        }
    }
}
