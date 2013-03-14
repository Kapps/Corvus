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
    /// A component to manage area of effect entities. Shouldnt be used by anything else.
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

            var aoe = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("AreaOfEffect").CreateEntity();
            //remove equipment component to avoid bugs.
            if (EquipmentComponent == null)
            {
                var aoeEC = aoe.GetComponent<EquipmentComponent>();
                aoe.Components.Remove(aoeEC);
            }
            aoe.Size = (EquipmentComponent == null) ? new Vector2(CPComponent.AoESize.X, CPComponent.AoESize.Y) 
                                                    : new Vector2(EquipmentComponent.CurrentWeapon.CombatProperties.AoESize.X, EquipmentComponent.CurrentWeapon.CombatProperties.AoESize.X);
            
            aoe.Position = new Vector2(entity.Location.Center.X - (aoe.Size.X / 2), entity.Location.Center.Y - (aoe.Size.Y / 2)); 
            entity.Scene.AddEntity(aoe);

            //set the sprite to draw.
            var spriteName = (EquipmentComponent == null) ? CPComponent.AoEName : EquipmentComponent.CurrentWeapon.CombatProperties.AoEName;
            var effect = CorvusGame.Instance.GlobalContent.LoadSprite(spriteName);
            var sc = aoe.GetComponent<SpriteComponent>();
            sc.Sprite = effect;

            //give it it's properties.
            if (EquipmentComponent != null)
            {
                var ec = aoe.GetComponent<EquipmentComponent>();
                ec.EquipWeapon(EquipmentComponent.CurrentWeapon);
            }
            var cpc = aoe.GetComponent<CombatPropertiesComponent>();
            cpc.CombatProperties = (EquipmentComponent == null) ? CPComponent.CombatProperties : EquipmentComponent.CurrentWeapon.CombatProperties;
            var ac = aoe.GetComponent<AttributesComponent>();
            ac.Attributes = AttributesComponent.Attributes;
            var se = aoe.GetComponent<StatusEffectPropertiesComponent>();
            se.StatusEffectAttributes = (EquipmentComponent == null) ? SEAComponent.StatusEffectAttributes : EquipmentComponent.CurrentWeapon.Effect;
            var aoec = aoe.GetComponent<AreaOfEffectComponent>();
            aoec.Classification = (EquipmentComponent == null) ? CPComponent.AoEHitableEntities : EquipmentComponent.CurrentWeapon.CombatProperties.AoEHitableEntities;

        }

    }
}
