using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Corvus.GameStates;
using Corvus;

namespace Corvus.Components
{
    /// <summary>
    /// A component to manage user input and the various states involved with the entity.
    /// </summary>
    public class PlayerControlComponent : Component
    {
        private string _DyingSprite = "";
        private float _DyingDuration = 0f;
        private string _DyingSound = "";
        private int _Level = 1;
        private int _CurrentExperience = 0;
        private int _ExperienceForNextLevel = 0;
        private float _RequiredExperienceCurve = 2f;
        private float _HealthGrowth = 0;
        private float _ManaGrowth = 0;
        private float _StrGrowth = 0;
        private float _DexGrowth = 0;
        private float _IntGrowth = 0;
        private bool _WantsToBlock = false;
        private bool _WantsToSwitchWeapon = false;
        private bool _IsPrev = false;
        private AttributesComponent AC;
        private MovementComponent MC;
        private CombatComponent CC;
        private PhysicsComponent PC;
        private SpriteComponent SC;
        private EquipmentComponent EC;
        private ScoreComponent ScoreComponent;

        /// <summary>
        /// Gets or sets the dying sprite.
        /// </summary>
        public string DyingSprite
        {
            get { return _DyingSprite; }
            set { _DyingSprite = value; }
        }

        /// <summary>
        /// Gets or sets the dying duration.
        /// </summary>
        public float DyingDuration
        {
            get { return _DyingDuration; }
            set { _DyingDuration = value; }
        }

        /// <summary>
        /// Gets or sets the sound effect to play when this entity dies.
        /// </summary>
        public string DyingSound
        {
            get { return _DyingSound; }
            set { _DyingSound = value; }
        }

        /// <summary>
        /// Gets or sets the current level.
        /// </summary>
        public int Level
        {
            get { return _Level; }
            set { _Level = Math.Max(value, 1); }
        }

        /// <summary>
        /// Gets or sets the current experience.
        /// </summary>
        public int CurrentExperience
        {
            get { return _CurrentExperience; }
            set
            {
                //no exp for the dead!
                if (AC == null || AC.IsDead)
                    return;
                int change = value - ExperienceForNextLevel;
                if (change < 0)
                    _CurrentExperience = value;
                else
                {
                    //level up.
                    int exp = 0;
                    do {
                        exp = change;
                        LevelUp();
                        change -= ExperienceForNextLevel;
                    } while (change > 0);
                    _CurrentExperience = exp;
                }
            }
        }

        /// <summary>
        /// Gets or sets the experience 
        /// </summary>
        public int ExperienceForNextLevel
        {
            get { return _ExperienceForNextLevel; }
            set { _ExperienceForNextLevel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating how much to multiply the next level required experience by.
        /// </summary>
        public float RequiredExperienceCurve
        {
            get { return _RequiredExperienceCurve; }
            set { _RequiredExperienceCurve = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets or sets the health growth.
        /// </summary>
        public float HealthGrowth
        {
            get { return _HealthGrowth; }
            set { _HealthGrowth = value; }
        }

        /// <summary>
        /// Gets or sets the mana growth.
        /// </summary>
        public float ManaGrowth
        {
            get { return _ManaGrowth; }
            set { _ManaGrowth = value; }
        }

        /// <summary>
        /// Gets or sets the strength growth.
        /// </summary>
        public float StrGrowth
        {
            get { return _StrGrowth; }
            set { _StrGrowth = value; }
        }

        /// <summary>
        /// Gets or sets the dexterity growth.
        /// </summary>
        public float DexGrowth
        {
            get { return _DexGrowth; }
            set { _DexGrowth = value; }
        }

        /// <summary>
        /// Gets or sets the intelligence growth.
        /// </summary>
        public float IntGrowth
        {
            get { return _IntGrowth; }
            set { _IntGrowth = value; }
        }

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
                
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AC = this.GetDependency<AttributesComponent>();
            MC = this.GetDependency<MovementComponent>();
            CC = this.GetDependency<CombatComponent>();
            PC = this.GetDependency<PhysicsComponent>();
            SC = this.GetDependency<SpriteComponent>();
            EC = this.GetDependency<EquipmentComponent>();
            ScoreComponent = this.GetDependency<ScoreComponent>();

            if (!AC.IsDiedRegistered)
                AC.Died += AC_Died;
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
            ScoreComponent.Coins = 0;

            AudioManager.PlaySoundEffect(DyingSound);
            DyingComponent.CreateDyingEntity(this.Parent, DyingSprite, DyingDuration);

            //Remove this if we ever have multiplayer, or just edit it to check for other players.
            //Lazy for now, since I doubt it'll happen.
            //CorvusGame.Instance.SceneManager.ReloadScenes();

            //If we dispose, the scene is gone apparently. Maybe? Doesn't really matter though.
            obj.Parent.Dispose();
        }
        
        private void LevelUp()
        {
            AudioManager.PlaySoundEffect("Cheer");
            var fc = this.GetDependency<FloatingTextComponent>();
            Level += 1;
            fc.Add("Level " + Level.ToString() + "!", Color.Gold);
            ExperienceForNextLevel = (int)(ExperienceForNextLevel * RequiredExperienceCurve);
            AC.MaxHealth += HealthGrowth;
            AC.MaxMana += ManaGrowth;
            AC.Strength += StrGrowth;
            AC.Dexterity += DexGrowth;
            AC.Intelligence += IntGrowth;
        }
    }
}
