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

        /// <summary>
        /// Equips a weapon and applies it's attributes. 
        /// </summary>
        public void EquipWeapon(string name, AttributesComponent ac)
        {
            DropCurrentWeapon();
            //Same weapon, no need to re-assign.
            if (name.Equals(CurrentWeapon.Name))
                return;
            CurrentWeapon = CreateWeapon(name, ac.Attributes);
        }

        /// <summary>
        /// Removes the weapon and sets it to the default weapon.
        /// </summary>
        public void RemoveWeapon()
        { 
            //Already is the default weapon. Don't drop weapon.
            if (CurrentWeapon.Name.Equals(DefaultWeaponName))
                return;

            DropCurrentWeapon();
            _CurrentWeapon = _DefaultWeapon;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AttributesComponent = Parent.GetComponent<AttributesComponent>();
            CombatComponent = Parent.GetComponent<CombatComponent>();

            //Creates the weapon from a blueprint.
            var weapon = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(DefaultWeaponName).CreateEntity();
            var attri = weapon.GetComponent<AttributesComponent>();
            _DefaultWeapon = CreateWeapon(DefaultWeaponName, attri.Attributes);
            _CurrentWeapon = _DefaultWeapon; 
            weapon.Dispose();
        }

        /// <summary>
        /// Creates a weapon with the specified attributes and effect.
        /// </summary>
        private Weapon CreateWeapon(string name, Attributes attributes)
        {
            var constructor = Helper.GetObjectConstructor<Weapon>(string.Format("Corvus.Components.Gameplay.Equipment.{0}", name), new Type[] { });
            var weapon = constructor();
            weapon.Attributes = attributes;
            return weapon;
        }

        private void DropCurrentWeapon()
        {
            var mc = this.GetDependency<MovementComponent>();
            var oldWeapon = EntityBlueprint.GetBlueprint(CurrentWeapon.Name).CreateEntity();
            var scene = Parent.Scene;
            int direction = (mc.CurrentDirection == CorvEngine.Direction.Left) ? 1 : -1;

            //TODO: maybe make it fly out instead of just dropping.
            oldWeapon.Position = new Vector2(Parent.Position.X + direction * (Parent.Size.X + 5), Parent.Position.Y - Parent.Size.Y - 5);
            oldWeapon.Size = new Vector2(32, 32); //TODO: Weapon size may change in the future

            scene.AddEntity(oldWeapon);
        }

        private AttributesComponent AttributesComponent;
        private CombatComponent CombatComponent;

    }
}
