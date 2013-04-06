using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorvEngine.Components;
using Corvus.Components.Gameplay.Equipment;
using Corvus.Components.Gameplay;
using CorvEngine.Components.Blueprints;
using Corvus.Components.Gameplay.StatusEffects;

namespace Corvus.Components
{
    /// <summary>
    /// A component to manage equipment.
    /// </summary>
    public class EquipmentComponent : Component
    {
        /// <summary>
        /// Gets the list of weapons.
        /// </summary>
        public WeaponCollection Weapons { get { return _Weapons; } }

        /// <summary>
        /// Gets the current weapon that is equipped.
        /// </summary>
        public Weapon CurrentWeapon { get { return Weapons[_CurrentWeaponIndex]; } }

        /// <summary>
        /// Gets the current weapon index.
        /// </summary>
        public int CurrentIndex { get { return _CurrentWeaponIndex; } }

        /// <summary>
        /// Gets or sets a value indicating whether to use weapon bonuses. Should be false for enemies, true for players.
        /// </summary>
        public bool UseWeaponBonuses
        {
            get { return _UseWeaponBonuses; }
            set { _UseWeaponBonuses = value; }
        }

        /// <summary>
        /// Gets or sets the number of weapons allowed. 
        /// </summary>
        public int Capacity
        {
            get { return _Capacity; }
            set { _Capacity = Math.Max(value, 1); }
        }

        /// <summary>
        /// Gets or sets the default weapon. Currently, defaults to Spear. 
        /// </summary>
        public string DefaultWeaponName
        {
            get { return _DefaultWeaponName; }
            set { _DefaultWeaponName = value; }
        }

        private WeaponCollection _Weapons = new WeaponCollection();
        private bool _UseWeaponBonuses = false;
        private int _Capacity = 3;
        private int _CurrentWeaponIndex = 0;
        private string _DefaultWeaponName;
        private Weapon _DefaultWeapon;

        /// <summary>
        /// Equips a new weapon.
        /// </summary>
        public void EquipWeapon(Weapon newWeapon)
        {
            //Already have a weapon of this type so replace the old one.
            if (Weapons.Contains(newWeapon.WeaponData.WeaponType))            
                ReplaceWeapon(Weapons[newWeapon.WeaponData.WeaponType], newWeapon);            
            //Full of weapons, so drop your currently equipped one.
            else if (Weapons.Count() >= Capacity)
                ReplaceWeapon(Weapons[CurrentWeapon.WeaponData.WeaponType], newWeapon);       
            //Regular, just add it.
            else
                Weapons.Add(newWeapon);
            _CurrentWeaponIndex = Weapons.Count() - 1;
        }

        /// <summary>
        /// Removes all the weapons from the player and gives them the default weapon.
        /// </summary>
        public void RemoveWeapons()
        {
            //Default weapon is the currently equiped one so do nothing.
            if (Weapons.Count() == 1 && CurrentWeapon.WeaponData.SystemName == DefaultWeaponName)
                return;
            Random rand = new Random();
            float launchMod = 1f;
            var wlist = Weapons.Take(Weapons.Count);
            foreach (Weapon w in Weapons.Reverse())
            {
                //drop the weapon except for the default one.
                if (w.WeaponData.SystemName != DefaultWeaponName)
                    DropWeapon(w.WeaponData.SystemName, launchMod);
                Weapons.Remove(w);
                launchMod += 0.25f;
            }
            _CurrentWeaponIndex = 0;
            Weapons.Add(_DefaultWeapon);
        }

        /// <summary>
        /// Switch which weapon to use.
        /// </summary>
        public void SwitchWeapon(bool getPrev)
        {
            if (Weapons.Count() == 1)
                return;

            _CurrentWeaponIndex += (getPrev) ? -1: 1;
            if (_CurrentWeaponIndex < 0)
                _CurrentWeaponIndex = Weapons.Count() - 1;
            else if (_CurrentWeaponIndex == Weapons.Count())
                _CurrentWeaponIndex = 0;

            CorvEngine.AudioManager.PlaySoundEffect("WeaponSwitch");
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            //_Weapons = new WeaponCollection();
            _CurrentWeaponIndex = 0;

            if(string.IsNullOrEmpty(DefaultWeaponName))
                return;
            if (Weapons.Count != 0)
                return;
            //Creates the weapon from a blueprint.
            var weapon = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(DefaultWeaponName).CreateEntity();
            var attri = weapon.GetComponent<AttributesComponent>();
            var props = weapon.GetComponent<CombatPropertiesComponent>();
            var data = weapon.GetComponent<WeaponPropertiesComponent>();
            var effect = weapon.GetComponent<StatusEffectPropertiesComponent>();
            _DefaultWeapon = new Weapon(data.WeaponData, props.CombatProperties, attri.Attributes, effect.StatusEffectAttributes);
            Weapons.Add(_DefaultWeapon);
            weapon.Dispose();
        }

        private void ReplaceWeapon(Weapon oldWeapon, Weapon newWeapon)
        {
            DropWeapon(oldWeapon.WeaponData.SystemName);
            Weapons.Remove(oldWeapon);
            Weapons.Add(newWeapon);
        }

        private void DropWeapon(string weaponToDrop, float launchModifier = 1f)
        {
            var mc = this.GetDependency<MovementComponent>();
            var scene = Parent.Scene;
            var oldWeapon = EntityBlueprint.GetBlueprint(weaponToDrop).CreateEntity();
            oldWeapon.Position = new Vector2(Parent.Position.X + -CorvusExtensions.GetSign(mc.CurrentDirection) * (Parent.Size.X + 5), Parent.Position.Y - Parent.Size.Y - 5);
            oldWeapon.Size = new Vector2(22, 22); 
            scene.AddEntity(oldWeapon);

            Random rand = new Random();
            float rMod = (float)rand.Next(20, 50);
            var pc = oldWeapon.GetComponent<PhysicsComponent>();
            pc.Velocity = new Vector2(-CorvusExtensions.GetSign(mc.CurrentDirection) * 225f * launchModifier + rMod, -325f * launchModifier + rMod);
        }

        private AttributesComponent AttributesComponent;

    }
}
