﻿using System;
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
            private set { _CurrentWeapon = value; }
        }

        private string _DefaultWeaponName = "Spear";
        private Weapon _DefaultWeapon;
        private Weapon _CurrentWeapon;
        private AttributesComponent AttributesComponent;
        private CombatComponent CombatComponent;

        /// <summary>
        /// Equips a weapon and applies it's attributes. 
        /// </summary>
        public void EquipWeapon(string name, AttributesComponent ac)
        {
            //Same weapon, no need to re-assign.
            if (name.Equals(CurrentWeapon.Name))
                return;
            //TODO: When equipping a new weapon, drop the old weapon so another player can pick it up or you can pick 
            //      it up if you don't like the new one.
            CurrentWeapon = CreateWeapon(name, ac.Attributes);
        }

        /// <summary>
        /// Removes the weapon and sets it to the default weapon.
        /// </summary>
        public void RemoveWeapon()
        { 
            //TODO: On weapon removal, through out your old weapon if it's not the default one.
            //Already is the default weapon.
            if (CurrentWeapon.Name.Equals(DefaultWeaponName))
                return;
            //TODO: Set it to the default weapon.
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = Parent.GetComponent<AttributesComponent>();
            CombatComponent = Parent.GetComponent<CombatComponent>();

            //TODO: Seems like a hackish way of getting a weapon entity.
            //      Also note that I'm assuming that the AllBlueprints field will always have ALL the blueprints. Not sure if it's ever cleared.
            var weapon = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(DefaultWeaponName).CreateEntity();
            _DefaultWeapon = CreateWeapon(DefaultWeaponName, weapon.GetComponent<AttributesComponent>().Attributes);
            _CurrentWeapon = _DefaultWeapon;
            weapon.Dispose();
        }

        /// <summary>
        /// Create a attributeless weapon.
        /// </summary>
        private Weapon CreateWeapon(string name)
        {
            var constructor = Helper.GetObjectConstructor<Weapon>(string.Format("Corvus.Components.Gameplay.Equipment.{0}", name), new Type[] { });
            var weapon = constructor();
            weapon.Attributes = new Attributes();
            return weapon;
        }

        /// <summary>
        /// Creates a weapon with the specified attributes.
        /// </summary>
        private Weapon CreateWeapon(string name, Attributes attributes)
        {
            Weapon weapon = CreateWeapon(name);
            weapon.Attributes = attributes;
            return weapon;
        }

    }
}
