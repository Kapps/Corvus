﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Corvus.Components.Gameplay
{
    //Word of caution: there is a class called System.Attribute. Don't get confused!
    /// <summary>
    /// A class that contains the attributes.
    /// </summary>
    public class Attributes
    {
        /// <summary>
        /// Gets or sets the attack range.
        /// </summary>
        public Vector2 MeleeAttackRange
        {
            get { return _MeleeAttackRange; }
            set { _MeleeAttackRange = value; }
        }

        /// <summary>
        /// Gets or sets the attack speed in milliseconds.
        /// </summary>
        public float AttackSpeed
        {
            get { return _AttackSpeed; }
            set { _AttackSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the amount of health that this component has.
        /// </summary>
        public float CurrentHealth
        {
            get { return _CurrentHealth; }
            set { _CurrentHealth = value; }
        }

        /// <summary>
        /// Gets or sets the maximum health allowed for this component.
        /// Setting this value also alters the current health to be equal to the old percentage of health.
        /// </summary>
        public float MaxHealth
        {
            get { return _MaxHealth; }
            set { _MaxHealth = value; }
        }

        /// <summary>
        /// Gets or sets the strength. Strength affects physical and ranged damage.
        /// </summary>
        public float Strength
        {
            get { return _Strength; }
            set { _Strength = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the strength modifer. Should be expressed as a percentage.
        /// </summary>
        public float StrModifier
        {
            get { return _StrModifier; }
            set { _StrModifier = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the Dexterity. Dexterity affects defense.
        /// </summary>
        public float Dexterity
        {
            get { return _Dexterity; }
            set { _Dexterity = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the dexterity modifier. Should be expressed as a percentage.
        /// </summary>
        public float DexModifier
        {
            get { return _DexModifier; }
            set { _DexModifier = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the intelligence. Intelligence affects elemental damage and ... not really sure yet.
        /// </summary>
        public float Intelligence
        {
            get { return _Intelligence; }
            set { _Intelligence = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the intelligence modifier. Should be expressed as a percentage.
        /// </summary>
        public float IntModifier
        {
            get { return _IntModifier; }
            set { _IntModifier = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the critical chance. Value must range from 0 to 1.0.
        /// </summary>
        public float CritChance
        {
            get { return _CritChance; }
            set { _CritChance = MathHelper.Clamp(value, 0, 1.0f); }
        }

        /// <summary>
        /// Gets or sets the critical damage. Value cannot be lower than 1.
        /// </summary>
        public float CritDamage
        {
            get { return _CritDamage; }
            set { _CritDamage = Math.Max(value, 1); }
        }

        /// <summary>
        /// Gets or sets a value that indicates how much damage is reduced while blocking. Value must range from 0 to 1.0.
        /// </summary>
        public float BlockDamageReduction
        {
            get { return _BlockDamageReduction; }
            set { _BlockDamageReduction = MathHelper.Clamp(value, 0f, 1f); }
        }

        private Vector2 _MeleeAttackRange = new Vector2();
        private float _AttackSpeed = 0f;
        private float _CurrentHealth = 100f;
        private float _MaxHealth = 100f;
        private float _Strength = 0;
        private float _StrModifier = 1f;
        private float _Dexterity = 0;
        private float _DexModifier = 1f;
        private float _Intelligence = 0;
        private float _IntModifier = 1f;
        private float _CritChance = 0f;
        private float _CritDamage = 0f;
        private float _BlockDamageReduction = 1f;
        
    }
}
