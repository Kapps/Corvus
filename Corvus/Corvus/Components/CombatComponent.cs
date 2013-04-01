using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Input;
using Corvus.Components.Gameplay;
using Corvus.Components.Gameplay.Equipment;

namespace Corvus.Components {
    /// <summary>
    /// A component that manages the combat.
    /// </summary>
	public class CombatComponent : Component {
        /// <summary>
        /// Gets or sets what can be attacked. 
        /// </summary>
        public EntityClassification AttackableEntities
        {
            get { return _AttackableEntities; }
            set { _AttackableEntities = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this entity is attacking using either melee or ranged.
        /// </summary>
        public bool IsAttacking
        {
            get { return _IsAttacking; }
            private set { _IsAttacking = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this entitiy is blocking.
        /// </summary>
        public bool IsBlocking
        {
            get { return _IsBlocking; }
            private set { _IsBlocking = value; }
        }

        private GameTime GameTime = new GameTime();
        private TimeSpan _AttackTimer = new TimeSpan(); //not sure if this is the best way to set up attack speed.
        private EntityClassification _AttackableEntities;
        private bool _IsAttacking = false;
        private bool _IsBlocking = false;
        private DateTime _LastBlock;
        private Action _AttackAction;
        private bool _StartedAttackFromGround = false;
        private Direction _AttackStartedDirection = Direction.None;
        private WeaponSwingAnimation _WeaponSwingAnimation = new WeaponSwingAnimation();
        private ShieldAnimation _ShieldAnimation;
        /// <summary>
        /// For Player only: Attack depending on whether the current weapon is melee or ranged.
        /// </summary>
        public void Attack()
        {
            //This guy is attacking, don't do anything.
            if (IsAttacking)
                return;

            //check if it consumes mana.
            if (CombatPropertiesComponent.ConsumesMana)
            {
                //Not enough mana so can't attack
                if (AttributesComponent.CurrentMana < AttributesComponent.ManaCost)
                    return;
                AttributesComponent.CurrentMana -= AttributesComponent.ManaCost;
                var ftc = this.GetDependency<FloatingTextComponent>();
                ftc.Add("-" + AttributesComponent.ManaCost.ToString(), Color.DarkBlue);
            }

            IsAttacking = true;

            //attacking while blocking cancels blocking
            if (IsBlocking)
                IsBlocking = false;

            //Check if attack began from the ground.
            _StartedAttackFromGround = PhysicsComponent.IsGrounded;
            //get the direction this attack started from.
            _AttackStartedDirection = MovementComponent.CurrentDirection;

            //Slow down move speed while attacking.
            if (PhysicsComponent.IsGrounded)
                MovementComponent.CombatWalkSpeedModifier = 0;//CombatPropertiesComponent.AttackSlowDown;
            
            //set animation
            float attackSpeed = AttributesComponent.AttackSpeed;
            SpriteComponent.Sprite.PlayAnimation(EquipmentComponent.CurrentWeapon.WeaponData.AnimationName + (MovementComponent.CurrentDirection == Direction.None ? "Down" : MovementComponent.CurrentDirection.ToString()), TimeSpan.FromMilliseconds(attackSpeed));
            if(EquipmentComponent.CurrentWeapon.WeaponData.IsMelee)
                _WeaponSwingAnimation.Start(this.Parent, "Sprites/Equipment/"+ EquipmentComponent.CurrentWeapon.WeaponData.SystemName, attackSpeed, new Vector2(-4f));

            //determine what type of attack to do.
            if (CombatPropertiesComponent.IsRanged)
                _AttackAction = AttackRanged;
            else
                _AttackAction = AttackMelee;
        }

        /// <summary>
        /// An attack meant to be used for AI.
        /// </summary>
        public void AttackAI()
        {
            //This guy is attacking, don't do anything.
            if (IsAttacking)
                return;

            var cpc = this.GetDependency<CombatPropertiesComponent>();
            IsAttacking = true;

            //Check if attack began from the ground.
            _StartedAttackFromGround = PhysicsComponent.IsGrounded;
            //get the direction this attack started from.
            _AttackStartedDirection = MovementComponent.CurrentDirection;

            //Slow down entity when it is attacking on the ground
            if (PhysicsComponent.IsGrounded)
                MovementComponent.CombatWalkSpeedModifier = 0;// CombatPropertiesComponent.AttackSlowDown;

            //TODO: A property to determine an attack animation for enemies. Could just have 'MeleeAttack' and every enemy needs that animation.
            float attackSpeed = AttributesComponent.AttackSpeed;
            SpriteComponent.Sprite.PlayAnimation("SpearAttack" + MovementComponent.CurrentDirection.ToString(), TimeSpan.FromMilliseconds(attackSpeed));
            if(cpc.EnemyUseWeaponAnimation)
                _WeaponSwingAnimation.Start(this.Parent, cpc.EnemyWeaponName, attackSpeed, cpc.EnemyWeaponOffset);

            if (!cpc.IsRanged)
                _AttackAction = EnemyAttackMelee;
            //TODO: put range attack here.
        }

        public void BeginBlock()
        {
            _ShieldAnimation.ShowEffect = true;
            if ((DateTime.Now - _LastBlock).TotalMilliseconds > AttributesComponent.BlockSpeed)
            {
                _LastBlock = DateTime.Now;

                //Stop walking when we start to block.
                MovementComponent.CombatWalkSpeedModifier = 0f;
                IsBlocking = true;

                //TODO: Get a proper animation.
                var Animation = SpriteComponent.Sprite.Animations["Block" + MovementComponent.CurrentDirection.ToString()];
                if (SpriteComponent.Sprite.ActiveAnimation != Animation)
                    SpriteComponent.Sprite.PlayAnimation(Animation.Name);
            } 
        }

        public void EndBlock()
        {
            _ShieldAnimation.ShowEffect = false;
            IsBlocking = false;
            ResumeAnimation();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            EquipmentComponent = Parent.GetComponent<EquipmentComponent>();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            MovementComponent = this.GetDependency<MovementComponent>();
            SpriteComponent = this.GetDependency<SpriteComponent>();
            PhysicsSystem = Parent.Scene.GetSystem<PhysicsSystem>();
            PhysicsComponent = Parent.GetComponent<PhysicsComponent>();
            CombatPropertiesComponent = this.GetDependency<CombatPropertiesComponent>();
            _ShieldAnimation = new ShieldAnimation(this.Parent, "Sprites/Misc/Misc_BlockSprite");
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            GameTime = Time;
            _WeaponSwingAnimation.Update(Time);
            if (IsAttacking)
            {
                _AttackTimer += Time.ElapsedGameTime;

                //XOR: jumping while attacking cancels attacking and landing while attacking cancels the attack.
                if (_StartedAttackFromGround == !PhysicsComponent.IsGrounded)
                {
                    IsAttacking = false;
                    _AttackAction = null;
                    _AttackTimer = TimeSpan.Zero;
                    _WeaponSwingAnimation.Stop();
                    ResumeAnimation();     
                }
               
                //attack occurs
                if(_AttackTimer >= TimeSpan.FromMilliseconds(AttributesComponent.AttackSpeed * CombatPropertiesComponent.HitDelay) && _AttackAction != null)
                {
                    _AttackAction();
                    _AttackAction = null;
                }
                //attack ends
                if (_AttackTimer >= TimeSpan.FromMilliseconds(AttributesComponent.AttackSpeed))
                {
                    IsAttacking = false;
                    ResumeAnimation();
                    _AttackTimer = TimeSpan.Zero;
                }
            }

            //Basically, if the player is walking and blocking, do blocking, which stops walking.
            if (IsBlocking)
                BeginBlock();

        }

        protected override void OnDraw()
        {
            base.OnDraw();
            _WeaponSwingAnimation.Draw();
            _ShieldAnimation.Draw();
        }

        private void AttackMelee()
        {
            AudioManager.PlaySoundEffect(CombatPropertiesComponent.AttackSound);
            //Enumerate over each entity that intersected with our attack rectangle, and if they're not us, make them take damage.
            foreach (var attackedEntity in PhysicsSystem.GetEntitiesAtLocation(CreateHitBox()).Reverse())
            {
                //might be a little inefficient because we have to keep searching a list to get the ClassificationComponent.
                var cc = attackedEntity.GetComponent<ClassificationComponent>();
                if (attackedEntity != Parent && cc.Classification == AttackableEntities)
                {
                    //TODO: Find a better way, if it's really needed. 
                    //A hack to get the Ai to react before taking damage.
                    var aic = attackedEntity.GetComponent<AIComponent>();
                    aic.Update(GameTime);

                    var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                    damageComponent.TakeDamage(AttributesComponent);

                    //Apply knockback.
                    var mc = attackedEntity.GetComponent<MovementComponent>();
                    mc.Knockback(AttributesComponent.TotalKnockback, CorvusExtensions.GetSign(MovementComponent.CurrentDirection));
                    
                    //Applies a status effect to the attacked enemy, if they can be affected.
                    var enemySEC = attackedEntity.GetComponent<StatusEffectsComponent>();
                    if (CombatPropertiesComponent.AppliesEffect && enemySEC != null)
                        enemySEC.ApplyStatusEffect(EquipmentComponent.CurrentWeapon.Effect);

                    if (CombatPropertiesComponent.IsAoE)
                        AreaOfEffectComponent.CreateAoEEntity(this.Parent);
                }
            }
        }

        private void AttackRanged()
        {
            if (!CombatPropertiesComponent.IsRanged)
                return;
            ProjectileComponent.CreateProjectileEntity(this.Parent, _AttackStartedDirection);
            //Play soundeffect
            AudioManager.PlaySoundEffect(CombatPropertiesComponent.AttackSound);
        }

        private void EnemyAttackMelee()
        {
            AudioManager.PlaySoundEffect(CombatPropertiesComponent.AttackSound);
            //Enumerate over each entity that intersected with our attack rectangle, and if they're not us, make them take damage.
            foreach (var attackedEntity in PhysicsSystem.GetEntitiesAtLocation(CreateHitBox()).Reverse())
            {
                //might be a little inefficient because we have to keep searching a list to get the ClassificationComponent.
                var cc = attackedEntity.GetComponent<ClassificationComponent>();
                if (attackedEntity != Parent && cc.Classification == AttackableEntities)
                {
                    var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                    damageComponent.TakeDamage(AttributesComponent);
                    
                    //knockback
                    var mc = attackedEntity.GetComponent<MovementComponent>();
                    mc.Knockback(AttributesComponent.TotalKnockback, CorvusExtensions.GetSign(MovementComponent.CurrentDirection));
                    
                    //When the enemy attacks, apply status effects, if any. 
                    if (CombatPropertiesComponent.AppliesEffect)
                    {
                        var seac = Parent.GetComponent<StatusEffectPropertiesComponent>();
                        if (seac == null)
                            continue;
                        var enemySEC = attackedEntity.GetComponent<StatusEffectsComponent>();
                        if (enemySEC == null)
                            continue;
                        enemySEC.ApplyStatusEffect(seac.StatusEffectAttributes);
                    }
                    if (CombatPropertiesComponent.IsAoE)
                        AreaOfEffectComponent.CreateAoEEntity(this.Parent);
                }
            }
        }

        private Rectangle CreateHitBox()
        {
            //For each entity that is contained within our attack rectangle, and that isn't us, apply damage.
            //The attack rectange is calculated using our centre, range, and half our height.
            int MeleeAttackRange = (int)AttributesComponent.MeleeAttackRange.X; //This is the number we use to calculate our attack rectangle's start x position and width. So basically, it's horizontal range.
            int attackHeight = (int)AttributesComponent.MeleeAttackRange.Y; //This is the number we use to calculate our attack rectangle's start y position and height. So basically, it's vertical range.

            Rectangle attackRectangle;
            if (_AttackStartedDirection == Direction.Left)
                attackRectangle = new Rectangle(Parent.Location.Center.X - MeleeAttackRange, Parent.Location.Y - attackHeight, MeleeAttackRange, Parent.Location.Height + attackHeight);
            else if (_AttackStartedDirection == Direction.Right)
                attackRectangle = new Rectangle(Parent.Location.Center.X, Parent.Location.Y - attackHeight, MeleeAttackRange, Parent.Location.Height + attackHeight);
            else
            {
                //Player hasn't moved yet or just isn't facing left or right.
                //Set their rectangle to themselves (anything that is inside them will be attacked). Until we figure out how best to handle it anyways.
                attackRectangle = Parent.Location;
            }
            return attackRectangle;
        }

        private void ResumeAnimation()
        {
            MovementComponent.CombatWalkSpeedModifier = 1f;
            if (MovementComponent.IsWalking)
                MovementComponent.BeginWalking(MovementComponent.CurrentDirection);
            else 
                MovementComponent.StopWalking();
        }

        private EquipmentComponent EquipmentComponent;
        private AttributesComponent AttributesComponent;
        private MovementComponent MovementComponent;
        private SpriteComponent SpriteComponent;
        private PhysicsSystem PhysicsSystem;
        private PhysicsComponent PhysicsComponent;
        private CombatPropertiesComponent CombatPropertiesComponent;
	}
}