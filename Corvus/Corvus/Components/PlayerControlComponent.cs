using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Input;

namespace Corvus.Components
{
    /// <summary>
    /// A component to manage user input and the various states involved with the entity.
    /// </summary>
    public class PlayerControlComponent : Component
    {
        private bool _WantsToBlock = false;
        private bool _WantsToSwitchWeapon = false;
        private bool _IsPrev = false;
        private AttributesComponent AC;
        private MovementComponent MC;
        private CombatComponent CC;
        private PhysicsComponent PC;
        private SpriteComponent SC;
        private EquipmentComponent EC;

        /// <summary>
        /// Gets the current direction.
        /// </summary>
        public Direction CurrentDirection { get { return MC.CurrentDirection; } }

        /// <summary>
        /// Moves the player in the specified direction. If it is attacking, the player is slowed. 
        /// </summary>
        public void BeginWalking(Direction dir)
        {
            if (!CC.IsAttacking)// && PC.IsGrounded)
                MC.BeginWalking(dir);
            else
            {
                MC.WalkDirection = dir;
                MC.CurrentDirection = MC.WalkDirection;
                MC.IsWalking = true;
            }
        }

        /// <summary>
        /// Stops the player's movement.
        /// </summary>
        public void StopWalking()
        {
            if (!CC.IsAttacking)
                MC.StopWalking();
            else
            {
                MC.WalkDirection = Direction.None;
                MC.IsWalking = false;
            }
        }

        /// <summary>
        /// Forces the player to jump. If it is blocking, cancels it.
        /// </summary>
        public void Jump(bool multijump)
        {
            MC.Jump(multijump);
            if (CC.IsBlocking)
                CC.EndBlock();
        }

        /// <summary>
        /// Forces the player to attack.
        /// </summary>
        public void Attack()
        {
            CC.Attack();
        }

        /// <summary>
        /// Force the player to block. The player will remain blocking until the button is released. 
        /// </summary>
        public void BeginBlock()
        {
            //can't block while in the middle of an attack or in the air
            if (!CC.IsAttacking && PC.IsGrounded)
                CC.BeginBlock();
            else
                _WantsToBlock = true;
        }

        /// <summary>
        /// Stops blocking.
        /// </summary>
        public void EndBlock()
        {
            _WantsToBlock = false;
            CC.EndBlock();
        }

        /// <summary>
        /// Switches weapon. 
        /// </summary>
        public void SwitchWeapon(bool getPrev)
        {
            //cant switch weapons when attacking or blocking
            if (!CC.IsAttacking && !CC.IsBlocking)
                EC.SwitchWeapon(getPrev);
            else
            {
                _WantsToSwitchWeapon = true; //only applies to attacking atm
                _IsPrev = getPrev;
            }
        }

        public PlayerControlComponent() :base()
        {
            AC.Died += AC_Died; //TODO: Not sure if best place to put this.
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AC = this.GetDependency<AttributesComponent>();
            MC = this.GetDependency<MovementComponent>();
            CC = this.GetDependency<CombatComponent>();
            PC = this.GetDependency<PhysicsComponent>();
            SC = this.GetDependency<SpriteComponent>();
            EC = this.GetDependency<EquipmentComponent>();
        }

        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time)
        {
            base.OnUpdate(Time);
            
            if (_WantsToBlock && (!CC.IsAttacking && PC.IsGrounded))
            {
                BeginBlock();
                _WantsToBlock = false;
            }
            if (_WantsToSwitchWeapon && (!CC.IsAttacking))
            {
                SwitchWeapon(_IsPrev);
                _WantsToSwitchWeapon = false;
            }
        }

        void AC_Died(AttributesComponent obj)
        {
            EC.RemoveWeapons();   
            obj.Parent.Dispose();
        }
    }
}
