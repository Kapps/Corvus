using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay;

namespace Corvus.Components {
	/// <summary>
	/// A class to manage attributes for this entity.
	/// </summary>
	public class AttributesComponent : Component {
		/// <summary>
		/// Gets an event called when the health of this Component runs out.
		/// </summary>
		public event Action<AttributesComponent> Died;
		/// <summary>
		/// Gets an event called when the amount of health this Component has remaining is changed.
		/// </summary>
		public event Action<AttributesComponent> CurrentHealthChanged;
		/// <summary>
		/// Gets an event called when the max health this Component has is changed.
		/// </summary>
		public event Action<AttributesComponent> MaxHealthChanged;
        
		private EquipmentComponent EquipmentComponent;
		private Attributes _Attributes = new Attributes();
        private bool _IsDead = false;

        /// <summary>
        /// Gets whether this entity is dead.
        /// </summary>
        public bool IsDead { get { return _IsDead; } }

        /// <summary>
        /// Gets a value indicating that the Died event has been registered.
        /// </summary>
        public bool IsDiedRegistered { get { return Died != null; } }
        
		/// <summary>
		/// Gets or sets this components Attributes.
		/// </summary>
		public Attributes Attributes { 
            get { return _Attributes; }
            set { _Attributes = value; }
        }

		/// <summary>
		/// Gets the overall attack power.
		/// </summary>
		public float Attack {
			get {
				if(!EquipmentComponent.UseWeaponBonuses)
					return Strength * StrModifier;
				return GetCombinedAttributeValues(Strength, StrModifier, EquipmentComponent.CurrentWeapon.Attributes.Strength, EquipmentComponent.CurrentWeapon.Attributes.StrModifier);
			}
		}

		/// <summary>
		/// Gets the overall defense.
		/// </summary>
		public float Defense {
			get {
                if (!EquipmentComponent.UseWeaponBonuses)
					return Dexterity * DexModifier;
				return GetCombinedAttributeValues(Dexterity, DexModifier, EquipmentComponent.CurrentWeapon.Attributes.Dexterity, EquipmentComponent.CurrentWeapon.Attributes.DexModifier);
			}
		}

        /// <summary>
        /// Gets the overall elemental power.
        /// </summary>
        public float ElementPower{
            get{
                if (!EquipmentComponent.UseWeaponBonuses)
                    return Intelligence * IntModifier;
                return GetCombinedAttributeValues(Intelligence, IntModifier, EquipmentComponent.CurrentWeapon.Attributes.Intelligence, EquipmentComponent.CurrentWeapon.Attributes.IntModifier);
            }
        }

		/// <summary>
		/// Gets the overall critical chance.
		/// </summary>
		public float CriticalChance {
			get {
                if (!EquipmentComponent.UseWeaponBonuses)
					return CritChance;
				return MathHelper.Clamp(CritChance + EquipmentComponent.CurrentWeapon.Attributes.CritChance, 0, 1f);
			}
		}

		/// <summary>
		/// Gets the overall critical damage.
		/// </summary>
		public float CriticalDamage {
			get {
                if (!EquipmentComponent.UseWeaponBonuses)
					return CritDamage;
				return CritDamage + EquipmentComponent.CurrentWeapon.Attributes.CritDamage;
			}
        }

        /// <summary>
        /// Gets the total knockback.
        /// </summary>
        public float TotalKnockback
        {
            get {
                if (!EquipmentComponent.UseWeaponBonuses)
                    return Knockback * KnockbackModifier;
                return EquipmentComponent.CurrentWeapon.Attributes.Knockback * KnockbackModifier;
            }
        }

		/// <summary>
		/// Gets or sets the attack range.
		/// </summary>
		public Vector2 MeleeAttackRange {
			get {
                if (!EquipmentComponent.UseWeaponBonuses)
					return Attributes.MeleeAttackRange;
				return EquipmentComponent.CurrentWeapon.Attributes.MeleeAttackRange;
			}
			set { Attributes.MeleeAttackRange = value; }
		}

		/// <summary>
		/// Gets or sets the attack speed in milliseconds.
		/// </summary>
		public float AttackSpeed {
			get {
                if (!EquipmentComponent.UseWeaponBonuses)
					return Attributes.AttackSpeed * AttackSpeedModifier;
                return EquipmentComponent.CurrentWeapon.Attributes.AttackSpeed * AttackSpeedModifier;
			}
			set { Attributes.AttackSpeed = value; }
		}

		/// <summary>
		/// Gets or sets a value that indicates how much damage is reduced while blocking. 
		/// </summary>
		public float BlockDamageReduction {
			get {
                if (!EquipmentComponent.UseWeaponBonuses)
					return Attributes.BlockDamageReduction;
				return EquipmentComponent.CurrentWeapon.Attributes.BlockDamageReduction;
			}
			set { Attributes.BlockDamageReduction = value; }
        }

        /// <summary>
        /// Gets or sets the knockback.
        /// </summary>
        public float Knockback
        {
            get {
                if (!EquipmentComponent.UseWeaponBonuses)
                    return Attributes.Knockback;
                return EquipmentComponent.CurrentWeapon.Attributes.Knockback;
            }
            set { Attributes.Knockback = value; }
        }

        /// <summary>
        /// Gets or sets the knockback modifier.
        /// </summary>
        public float KnockbackModifier
        {
            get { return Attributes.KnockbackModifier; }
            set { Attributes.KnockbackModifier = value; }
        }

        /// <summary>
        /// Gets or sets the elements this entity is resistant to.
        /// </summary>
        public Elements ResistantElements
        {
            get {
                if (!EquipmentComponent.UseWeaponBonuses)
                    return Attributes.ResistantElements;
                return EquipmentComponent.CurrentWeapon.Attributes.ResistantElements;
            }
            set { Attributes.ResistantElements = value; }
        }

        /// <summary>
        /// Gets or sets the elements this entity can attack with.
        /// </summary>
        public Elements AttackingElements
        {
            get {
                if (!EquipmentComponent.UseWeaponBonuses)
                    return Attributes.AttackingElements;
                return EquipmentComponent.CurrentWeapon.Attributes.AttackingElements;
            }
            set { Attributes.AttackingElements = value; }
        }

		/// <summary>
		/// Gets or sets the amount of health that this component has.
		/// </summary>
		public float CurrentHealth {
			get { return Attributes.CurrentHealth; }
			set {
                if (_IsDead && value > 0)
                    //TODO: Replace original setting. I removed cuz it was annoying when testing healing :P
                    return;//throw new NotSupportedException("Unable to edit current health of a dead HealthComponent.");
				value = Math.Min(MaxHealth, Math.Max(value, 0));
				Attributes.CurrentHealth = value;
				if(this.CurrentHealthChanged != null)
					CurrentHealthChanged(this);
				if(CurrentHealth == 0) {
					_IsDead = true;
					if(this.Died != null)
						this.Died(this);
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum health allowed for this component.
		/// Setting this value also alters the current health to be equal to the old percentage of health.
		/// </summary>
		public float MaxHealth {
			get { return Attributes.MaxHealth; }
			set {
				if(_IsDead && value > 0)
					throw new NotSupportedException("Unable to edit max health of a dead HealthComponent.");
				float CurrPercent = CurrentHealth / MaxHealth;
				Attributes.MaxHealth = value;
				if(MaxHealthChanged != null)
					MaxHealthChanged(this);
				CurrentHealth = MaxHealth * CurrPercent;
			}
		}

        /// <summary>
        /// Gets or sets the amount of mana that this component has.
        /// </summary>
        public float CurrentMana
        {
            get { return Attributes.CurrentMana; }
            set
            {
                value = Math.Min(MaxMana, Math.Max(value, 0));
                Attributes.CurrentMana = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum mana allowed for this component.
        /// Setting this value also alters the current mana to be equal to the old percentage of mana.
        /// </summary>
        public float MaxMana
        {
            get { return Attributes.MaxMana; }
            set
            {
                float currPercent = CurrentMana / MaxMana;
                Attributes.MaxMana = value;
                CurrentMana = MaxMana * currPercent;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates how much mana, expressed as a percentage of the max mana, to recover per second. 
        /// </summary>
        public float ManaRegen
        {
            get { return Attributes.ManaRegen; }
            set { Attributes.ManaRegen = value; }
        }

        /// <summary>
        /// How much mana to consume.
        /// Player Only.
        /// </summary>
        public float ManaCost
        {
            get { 
                if(!EquipmentComponent.UseWeaponBonuses)
                    return Attributes.ManaCost;
                return EquipmentComponent.CurrentWeapon.Attributes.ManaCost;
            }
            set { Attributes.ManaCost = value; }
        }

        /// <summary>
        /// Gets or sets the attack speed modifier.
        /// </summary>
        public float AttackSpeedModifier
        {
            get { return Attributes.AttackSpeedModifier; }
            set { Attributes.AttackSpeedModifier = value; }
        }

		/// <summary>
		/// Gets or sets the strength. Strength affects physical and ranged damage.
		/// </summary>
		public float Strength {
			get { return Attributes.Strength; }
			set { Attributes.Strength = value; }
		}

		/// <summary>
		/// Gets or sets the strength modifer. Should be expressed as a percentage.
		/// </summary>
		public float StrModifier {
			get { return Attributes.StrModifier; }
			set { Attributes.StrModifier = value; }
		}

		/// <summary>
		/// Gets or sets the Dexterity. Dexterity affects defense.
		/// </summary>
		public float Dexterity {
			get { return Attributes.Dexterity; }
			set { Attributes.Dexterity = value; }
		}

		/// <summary>
		/// Gets or sets the dexterity modifier. Should be expressed as a percentage.
		/// </summary>
		public float DexModifier {
			get { return Attributes.DexModifier; }
			set { Attributes.DexModifier = value; }
		}

		/// <summary>
		/// Gets or sets the intelligence. Intelligence affects elemental damage and resistance.
		/// </summary>
		public float Intelligence {
			get { return Attributes.Intelligence; }
			set { Attributes.Intelligence = value; }
		}

		/// <summary>
		/// Gets or sets the intelligence modifier. Should be expressed as a percentage.
		/// </summary>
		public float IntModifier {
			get { return Attributes.IntModifier; }
			set { Attributes.IntModifier = value; }
		}

		/// <summary>
		/// Gets or sets the critical chance. Value must range from 0 to 1.0.
		/// </summary>
		public float CritChance {
			get { return Attributes.CritChance; }
			set { Attributes.CritChance = value; }
		}

		/// <summary>
		/// Gets or sets the critical damage. Value cannot be lower than 1.
		/// </summary>
		public float CritDamage {
			get { return Attributes.CritDamage; }
			set { Attributes.CritDamage = value; }
		}

        /// <summary>
        /// Gets or sets the block chance.
        /// </summary>
        public float BlockChance
        {
            get { return Attributes.BlockChance; }
            set { Attributes.BlockChance = value; }
        }

        /// <summary>
        /// Gets or sets the block speed.
        /// </summary>
        public float BlockSpeed
        {
            get { return Attributes.BlockSpeed; }
            set { Attributes.BlockSpeed = value; }
        }

		/// <summary>
		/// Calculates the combined value of two attributes. (EX: this entitie's attributes plus it's equipment bonuses)
		/// </summary>
		private float GetCombinedAttributeValues(float v1, float m1, float v2, float m2) {
			float cAdd = v1 + v2;
			float cMult = m1 * m2;
			float result = cAdd * cMult;
			return result;
		}

        protected override void OnInitialize()
        {
            base.OnInitialize();
            EquipmentComponent = Parent.GetComponent<EquipmentComponent>();
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            if (!_IsDead && ManaRegen != 0)
            {
                CurrentMana += (MaxMana * ManaRegen) * (float)Time.ElapsedGameTime.Milliseconds / 1000f;
            }
        }
	}
}