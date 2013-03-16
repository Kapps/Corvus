using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Input;

namespace Corvus.Components
{
    public class PlayerControlComponent : Component
    {
        private bool _WantsToBlock = false;
        private bool _WantsToSwitchWeapon = false;
        private bool _IsPrev = false;
        private MovementComponent MC;
        private CombatComponent CC;
        private PhysicsComponent PC;
        private SpriteComponent SC;
        private EquipmentComponent EC;

        public Direction CurrentDirection { get { return MC.CurrentDirection; } }

        public void BeginWalking(Direction dir)
        {
            if (!CC.IsAttacking)
                MC.BeginWalking(dir);
            else
            {
                MC.WalkDirection = dir;
                MC.CurrentDirection = MC.WalkDirection;
                MC.IsWalking = true;
            }
        }

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

        public void Jump(bool multijump)
        {
            MC.Jump(multijump);
            if (CC.IsBlocking)
                CC.EndBlock();
        }

        public void Attack()
        {
            CC.Attack();
        }

        public void BeginBlock()
        {
            //can't block while in the middle of an attack or in the air
            if (!CC.IsAttacking && PC.IsGrounded)
                CC.BeginBlock();
            else
                _WantsToBlock = true;
        }

        public void EndBlock()
        {
            _WantsToBlock = false;
            CC.EndBlock();
        }

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

        protected override void OnInitialize()
        {
            base.OnInitialize();
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
            if (_WantsToSwitchWeapon && (!CC.IsAttacking))// && !CC.IsBlocking))
            {
                SwitchWeapon(_IsPrev);
                _WantsToSwitchWeapon = false;
            }

        }
    }
}
