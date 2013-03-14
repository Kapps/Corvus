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
    /// A component to manage the area of effect entity. Shouldnt be used by anything else.
    /// </summary>
    public class AreaOfEffectComponent : CollisionEventComponent
    {
        private AttributesComponent AttributesComponent;
        private StatusEffectPropertiesComponent SEAComponent;
        private CombatPropertiesComponent CPComponent;
        private TimeSpan _Timer = new TimeSpan();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CPComponent = this.GetDependency<CombatPropertiesComponent>();
            SEAComponent = Parent.GetComponent<StatusEffectPropertiesComponent>();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            _Timer = TimeSpan.Zero;
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            _Timer += Time.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(CPComponent.AoEDuration))
            {
                Parent.Dispose();
            }
        }

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            bool colGood = false;
            //Apply normal damage.
            var dc = Entity.GetComponent<DamageComponent>();
            if (dc != null)
            {
                dc.TakeDamage(AttributesComponent, CPComponent.AoEDamagePercent);
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
            return colGood;
        }
        
        //TODO: Totally a hacky method of doing this.
        /// <summary>
        /// Creates a explosions around the specified entity.
        /// </summary>
        public static void CreateAoEEntity(Entity entity)
        {
            var CPComponent = entity.GetComponent<CombatPropertiesComponent>();
            var SEAComponent = entity.GetComponent<StatusEffectPropertiesComponent>();
            var AttributesComponent = entity.GetComponent<AttributesComponent>();
            var EquipmentComponent = entity.GetComponent<EquipmentComponent>();
            var MovementComponent = entity.GetComponent<MovementComponent>();

            var aoe = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("AreaOfEffect").CreateEntity();
            aoe.Size = new Vector2(CPComponent.AoESize.X, CPComponent.AoESize.Y);
            var center = entity.Location.Center;
            aoe.Position = new Vector2(center.X - (aoe.Size.X / 2), center.Y - (aoe.Size.Y / 2));
            entity.Scene.AddEntity(aoe);
            var spriteName = CPComponent.AoEName;
            var effect = CorvusGame.Instance.GlobalContent.LoadSprite(spriteName);
            var sc = aoe.GetComponent<SpriteComponent>();
            sc.Sprite = effect;
            //give it it's properties.
            if (EquipmentComponent.UseWeaponBonuses)
            {
                var ec = aoe.GetComponent<EquipmentComponent>();
                ec.UseWeaponBonuses = true;
                ec.EquipWeapon(EquipmentComponent.CurrentWeapon);
                var se = aoe.GetComponent<StatusEffectPropertiesComponent>();
                se.StatusEffectAttributes = EquipmentComponent.CurrentWeapon.Effect;
            }
            else
            {
                var se = aoe.GetComponent<StatusEffectPropertiesComponent>();
                se.StatusEffectAttributes = SEAComponent.StatusEffectAttributes;
            }
            var cpc = aoe.GetComponent<CombatPropertiesComponent>();
            cpc.CombatProperties = CPComponent.CombatProperties;
            var ac = aoe.GetComponent<AttributesComponent>();
            ac.Attributes = AttributesComponent.Attributes;
            var aoec = aoe.GetComponent<AreaOfEffectComponent>();
            aoec.Classification = CPComponent.AoEHitableEntities;
        }

    }
}
