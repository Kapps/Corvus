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
        private CombatComponent CombatComponent;
        private MovementComponent MovementComponent;
        private FloatingTextComponent FloatingTextComponent;

        /// <summary>
        /// Applies static damage with only normal rule.
        /// </summary>
        public void TakeDamage(Entity attacker, float incomingDamage, float modifier = 1f){
            float blockMultipler = BlockDamageReduction(attacker);
            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, incomingDamage);
            float overallDamage = damageTaken * blockMultipler * modifier;
            AttributesComponent.CurrentHealth -= overallDamage;

            FloatingTextComponent.Add(overallDamage, Color.White);
        }

        /// <summary>
        /// Applies damage, with the normal rules, based on the attacker's attributes.
        /// </summary>
        public void TakeDamage(AttributesComponent attacker, float modifier = 1f){
            float blockMultipler = BlockDamageReduction(attacker.Parent);
            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, attacker.Attack);
            float criticalMultiplier = CriticalDamageChance(attacker.CriticalChance, attacker.CriticalDamage);
            float elementalMultiplier = ElementalDamage(AttributesComponent.ResistantElements, AttributesComponent.ElementPower, attacker.AttackingElements, attacker.ElementPower);
            float overallDamage = damageTaken * criticalMultiplier * blockMultipler * elementalMultiplier * modifier;
            AttributesComponent.CurrentHealth -= overallDamage;

            if (criticalMultiplier != 1) //critical hit.
                FloatingTextComponent.Add(overallDamage, Color.Orange);
            else if (elementalMultiplier > 1) //entity is weak to that element
                FloatingTextComponent.Add(overallDamage, Color.Crimson);
            else if (elementalMultiplier < 1) //entity is resistant to that element
                FloatingTextComponent.Add(overallDamage, Color.Navy);
            else
                FloatingTextComponent.Add(overallDamage, Color.White);
        }

        private float NormalDamageFormula(float myDefense, float incomingDamage){
            return Math.Max(incomingDamage - (myDefense * 0.70f), 1);
        }

        private float CriticalDamageChance(float critChance, float critDamage){
            Random rand = new Random();
            return (rand.NextDouble() <= critChance) ? critDamage : 1f;
        }

        private float BlockDamageReduction(Entity attacker)
        {
            if (!CombatComponent.IsBlocking)
                return 1f;
            else
            {
                var myLoc = this.Parent.Location.Center.X;
                var attackerLoc = attacker.Location.Center.X;
                var pos = attackerLoc - myLoc;
                if ((MovementComponent.CurrentDirection == CorvEngine.Direction.Right && pos > 0) ||
                    (MovementComponent.CurrentDirection == CorvEngine.Direction.Left && pos < 0))
                    return AttributesComponent.BlockDamageReduction;
            }
            return 1f;
        }
        
        /// <summary>
        /// Basically. Fire < Water < Earth < Wind < Fire.... Physical reduces Phyiscal by 75%.
        /// </summary>
        private float ElementalDamage(Elements res, float myInt, Elements att, float attackerInt)
        {
            if (res == Elements.None || att == Elements.None)
                return 1f;
            
            float multiplier = 1f;
            if (res == Elements.Physical && att == Elements.Physical)
                multiplier -= 0.75f;
            else if ((res == Elements.Fire && att == Elements.Water) ||
                (res == Elements.Water && att == Elements.Earth) ||
                (res == Elements.Earth && att == Elements.Wind) ||
                (res == Elements.Wind && att == Elements.Fire))
            {
                multiplier += 0.5f;
                multiplier += (attackerInt / (attackerInt + myInt));
            }
            else if ((att == Elements.Fire && res == Elements.Water) ||
                (att == Elements.Water && res == Elements.Earth) ||
                (att == Elements.Earth && res == Elements.Wind) ||
                (att == Elements.Wind && res == Elements.Fire))
            {
                multiplier -= 0.5f;
                multiplier -= (myInt / (2f * (myInt + attackerInt)));
            }
            else if (res == Elements.All && (att == Elements.Fire || att == Elements.Water ||
                    att == Elements.Earth || att == Elements.Wind))
            {
                multiplier -= 0.75f;
                multiplier -= (myInt / (4f * (myInt + attackerInt)));
            }
            return multiplier;
        }

        protected override void OnInitialize(){
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            CombatComponent = Parent.GetComponent<CombatComponent>();
            MovementComponent = this.GetDependency<MovementComponent>();
            FloatingTextComponent = this.GetDependency<FloatingTextComponent>();
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            FloatingTextComponent.Update(Time);
        }
    }

    
}