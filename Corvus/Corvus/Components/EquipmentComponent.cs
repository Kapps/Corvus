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

        private WeaponCollection _Weapons;
        private int _Capacity = 3;
        private int _CurrentWeaponIndex = 0;
        private string _DefaultWeaponName = "Spear";
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
            //TODO: Actually test this. Note that when dropping weapons, they will all clump together because the flight distance isnt implemented yet.
            
            //Default weapon is the currently equiped one so do nothing.
            if (Weapons.Count() == 1 && CurrentWeapon.WeaponData.Name == DefaultWeaponName)
                return;
            foreach (Weapon w in Weapons.Reverse())
            {
                //drop the weapon except for the default one.
                if (w.WeaponData.Name != DefaultWeaponName)
                    DropWeapon(w.WeaponData.Name);
                Weapons.Remove(w);
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
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            _Weapons = new WeaponCollection();

            //Creates the weapon from a blueprint.
            var weapon = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(DefaultWeaponName).CreateEntity();
            var attri = weapon.GetComponent<AttributesComponent>();
            var data = weapon.GetComponent<WeaponDataComponent>();
            _DefaultWeapon = new Weapon(data.WeaponData, attri.Attributes);
            Weapons.Add(_DefaultWeapon);
            weapon.Dispose();
        }

        private void ReplaceWeapon(Weapon oldWeapon, Weapon newWeapon)
        {
            DropWeapon(oldWeapon.WeaponData.Name);
            Weapons.Remove(oldWeapon);
            Weapons.Add(newWeapon);
        }

        private void DropWeapon(string weaponToDrop)
        {
            var mc = this.GetDependency<MovementComponent>();
            var scene = Parent.Scene;
            var oldWeapon = EntityBlueprint.GetBlueprint(weaponToDrop).CreateEntity();
            oldWeapon.Position = new Vector2(Parent.Position.X + -CorvusExtensions.GetSign(mc.CurrentDirection) * (Parent.Size.X + 5), Parent.Position.Y - Parent.Size.Y - 5);
            oldWeapon.Size = new Vector2(32, 32); //TODO: Weapon size may change in the future

            scene.AddEntity(oldWeapon);
        }
        //private void DropCurrentWeapon()
        //{
        //    var mc = this.GetDependency<MovementComponent>();
        //    var oldWeapon = EntityBlueprint.GetBlueprint(CurrentWeapon.WeaponType).CreateEntity();
        //    var scene = Parent.Scene;
        //    int direction = (mc.CurrentDirection == CorvEngine.Direction.Left) ? 1 : -1;

        //    //TODO: maybe make it fly out instead of just dropping.
        //    oldWeapon.Position = new Vector2(Parent.Position.X + direction * (Parent.Size.X + 5), Parent.Position.Y - Parent.Size.Y - 5);
        //    oldWeapon.Size = new Vector2(32, 32); //TODO: Weapon size may change in the future

        //    scene.AddEntity(oldWeapon);
        //}

        private AttributesComponent AttributesComponent;

        ///// <summary>
        ///// Gets or sets the default weapon. Currently, defaults to Spear. 
        ///// </summary>
        //public string DefaultWeaponName
        //{
        //    get { return _DefaultWeaponName; }
        //    set { _DefaultWeaponName = value; }
        //}

        ///// <summary>
        ///// Gets the current weapon.
        ///// </summary>
        //public Weapon CurrentWeapon
        //{
        //    get { return _CurrentWeapon; }
        //    private set { _CurrentWeapon = value; }
        //}
        
        //private string _DefaultWeaponName = "Spear";
        //private Weapon _CurrentWeapon;
        //private Weapon _DefaultWeapon;

        ///// <summary>
        ///// Equips a weapon and applies it's attributes. 
        ///// </summary>
        //public void EquipWeapon(string name, AttributesComponent ac)
        //{
        //    DropCurrentWeapon();
        //    //Same weapon, no need to re-assign.
        //    if (name.Equals(CurrentWeapon.Name))
        //        return;
        //    CurrentWeapon = CreateWeapon(name, ac.Attributes);
        //}

        ///// <summary>
        ///// Removes the weapon and sets it to the default weapon.
        ///// </summary>
        //public void RemoveWeapon()
        //{ 
        //    //Already is the default weapon. Don't drop weapon.
        //    if (CurrentWeapon.Name.Equals(DefaultWeaponName))
        //        return;

        //    DropCurrentWeapon();
        //    _CurrentWeapon = _DefaultWeapon;
        //}

        //protected override void OnInitialize()
        //{
        //    base.OnInitialize();
        //    AttributesComponent = Parent.GetComponent<AttributesComponent>();
        //    CombatComponent = Parent.GetComponent<CombatComponent>();

        //    //Creates the weapon from a blueprint.
        //    var weapon = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(DefaultWeaponName).CreateEntity();
        //    var attri = weapon.GetComponent<AttributesComponent>();
        //    _DefaultWeapon = CreateWeapon(DefaultWeaponName, attri.Attributes);
        //    _CurrentWeapon = _DefaultWeapon; 
        //    weapon.Dispose();
        //}

        ///// <summary>
        ///// Creates a weapon with the specified attributes and effect.
        ///// </summary>
        //private Weapon CreateWeapon(string name, Attributes attributes)
        //{
        //    var constructor = Helper.GetObjectConstructor<Weapon>(string.Format("Corvus.Components.Gameplay.Equipment.{0}", name), new Type[] { });
        //    var weapon = constructor();
        //    weapon.Attributes = attributes;
        //    return weapon;
        //}

        //private void DropCurrentWeapon()
        //{
        //    var mc = this.GetDependency<MovementComponent>();
        //    var oldWeapon = EntityBlueprint.GetBlueprint(CurrentWeapon.Name).CreateEntity();
        //    var scene = Parent.Scene;
        //    int direction = (mc.CurrentDirection == CorvEngine.Direction.Left) ? 1 : -1;

        //    //TODO: maybe make it fly out instead of just dropping.
        //    oldWeapon.Position = new Vector2(Parent.Position.X + direction * (Parent.Size.X + 5), Parent.Position.Y - Parent.Size.Y - 5);
        //    oldWeapon.Size = new Vector2(32, 32); //TODO: Weapon size may change in the future

        //    scene.AddEntity(oldWeapon);
        //}

        //private AttributesComponent AttributesComponent;
        //private CombatComponent CombatComponent;

    }
}
