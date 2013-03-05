using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay;

namespace Corvus.Components{
    /// <summary>
    /// Handles the damage events for this entity.
    /// </summary>
    public class DamageComponent : Component{
        private AttributesComponent AttributesComponent;
        private FloatingTextList _FloatingTexts = new FloatingTextList();

        /// <summary>
        /// Applies damage with only normal rule.
        /// </summary>
        public void TakeDamage(float incomingDamage){
            var cc = Parent.GetComponent<CombatComponent>();

            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, incomingDamage, cc.IsBlocking);
            AttributesComponent.CurrentHealth -= damageTaken;

            _FloatingTexts.AddFloatingTexts(damageTaken, Color.White);
        }

        /// <summary>
        /// Applies damage, with the normal rules, based on the attacker's attributes.
        /// </summary>
        public void TakeDamage(AttributesComponent attacker){
            var cc = Parent.GetComponent<CombatComponent>();

            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, attacker.Attack, cc.IsBlocking);
            float criticalMultiplier = CriticalDamageChance(attacker.CriticalChance, attacker.CriticalDamage);
            float overallDamage = damageTaken * criticalMultiplier;
            AttributesComponent.CurrentHealth -= overallDamage;

            if (criticalMultiplier != 1)
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.Orange);
            else
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.White);
        }

        //TODO: Fix blocking calculations. Now it just halves all damage.
        private float NormalDamageFormula(float myDefense, float incomingDamage, bool isBlocking){
            if (isBlocking)
            {
                return Math.Max(incomingDamage - (myDefense * 0.70f), 1)/2;
            }
            else
            {
                return Math.Max(incomingDamage - (myDefense * 0.70f), 1);
            }
        }

        private float CriticalDamageChance(float critChance, float critDamage){
            Random rand = new Random();
            return (rand.NextDouble() <= critChance) ? critDamage : 1f;
        }

        protected override void OnInitialize(){
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
        }

        protected override void OnUpdate(GameTime Time){
            base.OnUpdate(Time);
            if (_FloatingTexts.Count == 0)
                return;
            var width = Parent.Size.X;
            var position = Parent.Position + new Vector2(width / 2, 0);
            var ToScreen = Camera.Active.ScreenToWorld(position);
            _FloatingTexts.Update(Time, ToScreen);
        }

        protected override void OnDraw(){
            base.OnDraw();
            if (_FloatingTexts.Count == 0)
                return;
            _FloatingTexts.Draw();
        }
    }

    
}