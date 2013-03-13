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
    /// A component used to handle the collision events for projectiles ONLY.
    /// </summary>
    public class CollisionProjectileComponent : CollisionEventComponent
    {
        private AttributesComponent AttributesComponent;
        private StatusEffectPropertiesComponent SEAComponent;
        private CombatPropertiesComponent CPComponent;
                
        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            bool colGood = false;
            //Apply normal damage.
            var dc = Entity.GetComponent<DamageComponent>();
            if (dc != null){
                dc.TakeDamage(AttributesComponent);
                colGood = true;
            }
            //Apply status effect if it can.
            if (CPComponent.AppliesEffect){
                var sc = Entity.GetComponent<StatusEffectsComponent>();
                if (sc != null){
                    sc.ApplyStatusEffect(SEAComponent.StatusEffectAttributes);
                    colGood = true;
                }
            }
            //Make an aoe appear if there should be one.
            if (CPComponent.IsAoE)
            {
                //var aoe = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("AreaOfEffect").CreateEntity();
                //aoe.Position = new Vector2(Parent.Location.Center.X, Parent.Location.Center.Y); //TODO: FIX. THIS IS ACTUALLY WRONG.
                //aoe.Size = new Vector2(CPComponent.AoESize.X, CPComponent.AoESize.Y); 
                //Parent.Scene.AddEntity(aoe);

                ////set the sprite to draw.
                //var effect = CorvusGame.Instance.GlobalContent.LoadSprite(CPComponent.AoEName);
                //var sc = aoe.GetComponent<SpriteComponent>();
                //sc.Sprite = effect;

                ////give it it's properties.
                //var cpc = aoe.GetComponent<CombatPropertiesComponent>();
                //cpc.CombatProperties = CPComponent.CombatProperties;
                //var ac = aoe.GetComponent<AttributesComponent>();
                //ac.Attributes = AttributesComponent.Attributes;
                //var se = aoe.GetComponent<StatusEffectPropertiesComponent>();
                //se.StatusEffectAttributes = SEAComponent.StatusEffectAttributes;

            }

            return colGood;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            SEAComponent = Parent.GetComponent<StatusEffectPropertiesComponent>();
            CPComponent = this.GetDependency<CombatPropertiesComponent>();
        }
    }
}
