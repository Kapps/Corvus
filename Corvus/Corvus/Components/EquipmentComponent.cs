using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorvEngine.Components;
using Corvus.Components.Gameplay.Equipment;
using Corvus.Components.Gameplay;

namespace Corvus.Components
{
    /// <summary>
    /// A component to manage equipment.
    /// </summary>
    public class EquipmentComponent : Component
    {
        /// <summary>
        /// Gets or sets the default weapon. Default weapon gives no bonuses. Currently, defaults to Spear. 
        /// </summary>
        public string DefaultWeaponName
        {
            get { return _DefaultWeaponName; }
            set { _DefaultWeaponName = value; }
        }

        /// <summary>
        /// Gets the current weapon.
        /// </summary>
        public Weapon CurrentWeapon
        {
            get { return _CurrentWeapon; }
            private set 
            {
                if (value != null && value.Name != _CurrentWeapon.Name)
                {
                    RemoveBonuses();
                    _CurrentWeapon = value;
                    ApplyBonuses();
                }
            }
        }

        private Weapon _DefaultWeapon;
        private string _DefaultWeaponName = "Spear";
        private Weapon _CurrentWeapon;
        private AttributesComponent AttributesComponent;
        private CombatComponent CombatComponent;

        /// <summary>
        /// Equips a weapon and applies it's attributes. 
        /// </summary>
        public void EquipWeapon(string name, AttributesComponent ac)
        {
            //Same weapon, no need to reassign.
            //if (name.Equals(CurrentWeapon.Name))
            //    return;
            Type type = Type.GetType(string.Format("Corvus.Components.Gameplay.Equipment.{0}", name));
            var equip = Helper.GetObject<Weapon>(type);
            equip.StrModifier = ac.StrModifier;
            equip.DexModifier = ac.DexModifier;
            equip.IntModifier = ac.IntModifier;
            equip.CritChanceModifier = ac.CritChanceModifier;
            equip.CritDamageModifier = ac.CritDamageModifier;
            CurrentWeapon = equip;
        }

        public void RemoveWeapon()
        { 
            //TODO: On weapon removal, replace it with the default weapon.
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = Parent.GetComponent<AttributesComponent>();
            CombatComponent = Parent.GetComponent<CombatComponent>();

            //sets default weapon
            Type type = Type.GetType(string.Format("Corvus.Components.Gameplay.Equipment.{0}", DefaultWeaponName));
            var equip = Helper.GetObject<Weapon>(type);
            _DefaultWeapon = equip;
            _CurrentWeapon = equip;
            ApplyBonuses();
        }

        private void ApplyBonuses()
        {
            AttributesComponent.StrModifier *= CurrentWeapon.StrModifier;
            AttributesComponent.DexModifier *= CurrentWeapon.DexModifier;
            AttributesComponent.IntModifier *= CurrentWeapon.IntModifier;
            AttributesComponent.CritChanceModifier += CurrentWeapon.CritChanceModifier;
            AttributesComponent.CritDamageModifier += CurrentWeapon.CritDamageModifier;
            CombatComponent.AttackAnimation = CurrentWeapon.AnimationName;
        }

        private void RemoveBonuses()
        {
            AttributesComponent.StrModifier /= CurrentWeapon.StrModifier;
            AttributesComponent.DexModifier /= CurrentWeapon.DexModifier;
            AttributesComponent.IntModifier /= CurrentWeapon.IntModifier;
            AttributesComponent.CritChanceModifier -= CurrentWeapon.CritChanceModifier;
            AttributesComponent.CritDamageModifier -= CurrentWeapon.CritDamageModifier;
            CombatComponent.AttackAnimation = CurrentWeapon.AnimationName; //
        }
    }
}
