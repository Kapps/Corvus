using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Input;

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
        /// Gets or sets a value indicating that this entity is attacking using melee.
        /// </summary>
        public bool IsAttackingMelee
        {
            get { return _IsAttackingMelee; }
            private set { _IsAttackingMelee = value; }
        }

        /// <summary>
        /// Get or sets a value indicating that this entity is attacking using range.
        /// </summary>
        public bool IsAttackingRanged
        {
            get { return _IsAttackingRanged; }
            set { _IsAttackingRanged = value; }
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
        private bool _IsAttackingMelee = false;
        private bool _IsAttackingRanged = false;
        private bool _IsBlocking = false;
        private DateTime _LastBlock;

        /// <summary>
        /// Attack depending on whether the current weapon is melee or ranged.
        /// </summary>
        public void Attack()
        {
            if (EquipmentComponent.CurrentWeapon.WeaponData.IsRanged)
                AttackRanged();
            else
                AttackMelee();
        }

        /// <summary>
        /// An attack meant to be used for AI, as they lack equipment components.
        /// </summary>
        public void AttackAI()
        {
            //This guy is attacking, don't do anything.
            if (_IsAttackingMelee)
                return;
            _IsAttackingMelee = true;

            //TODO: A property to determine an attack animation for enemies. Could just have 'MeleeAttack' and every enemy needs that animation.
            float attackSpeed = AttributesComponent.AttackSpeed;
            SpriteComponent.Sprite.PlayAnimation("SpearAttack" + MovementComponent.CurrentDirection.ToString(), TimeSpan.FromMilliseconds(attackSpeed));

            //Enumerate over each entity that intersected with our attack rectangle, and if they're not us, make them take damage.
            foreach (var attackedEntity in PhysicsSystem.GetEntitiesAtLocation(CreateHitBox()))
            {
                //might be a little inefficient because we have to keep searching a list to get the ClassificationComponent.
                var cc = attackedEntity.GetComponent<ClassificationComponent>();
                if (attackedEntity != Parent && cc.Classification == AttackableEntities)
                {
                    var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                    damageComponent.TakeDamage(AttributesComponent);

                    //TODO: Need a property to determine if the enemy can apply status effects through attacks.
                    //When the enemy attacks, apply status effects, if any. 
                    var seac = Parent.GetComponent<StatusEffectAttributesComponent>();
                    if (seac == null)
                        continue;
                    var enemySEC = attackedEntity.GetComponent<StatusEffectsComponent>();
                    if (enemySEC == null)
                        continue;
                    enemySEC.ApplyStatusEffect(seac.StatusEffectAttributes);
                }
            }
        }

        /// <summary>
        /// Attacks an enemy with a close range attack.
        /// </summary>
		private void AttackMelee() {
            //This guy is attacking, don't do anything.
            if (IsAttackingMelee)
                return;
            IsAttackingMelee = true;
            // TODO: Maybe the attack should start at a specific frame.
			// TODO: Limit number of attacks they can do.
			// TODO: Decide on how best to integrate things that are mutually exclusive, like attacking while walking.
			// At the very least the sprites for it will be mutually exclusive.
            float attackSpeed = AttributesComponent.AttackSpeed;
            SpriteComponent.Sprite.PlayAnimation(EquipmentComponent.CurrentWeapon.WeaponData.AnimationName + (MovementComponent.CurrentDirection == Direction.None ? "Down" : MovementComponent.CurrentDirection.ToString()), TimeSpan.FromMilliseconds(attackSpeed));

            //Enumerate over each entity that intersected with our attack rectangle, and if they're not us, make them take damage.
            foreach (var attackedEntity in PhysicsSystem.GetEntitiesAtLocation(CreateHitBox()))
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

                    //Applies a status effect to the attacked enemy, if they can be affected.
                    var enemySEC = attackedEntity.GetComponent<StatusEffectsComponent>();
                    if (EquipmentComponent.CurrentWeapon.WeaponData.AppliesEffect && enemySEC != null)
                        enemySEC.ApplyStatusEffect(EquipmentComponent.CurrentWeapon.Effect);
                }
            }
		}

        private void AttackRanged()
        {
            if (IsAttackingRanged)
                return;
            IsAttackingRanged = true;
            
            //Create entity
            var weaponData = EquipmentComponent.CurrentWeapon.WeaponData;
            var projectile = CreateProjectileEntity(weaponData.ProjectileName, weaponData.ProjectileVelocity);

            //A bit of a hack to get the projectile to apply damage and status effects.
            var ac = projectile.GetComponent<AttributesComponent>();
            ac.Attributes = AttributesComponent.Attributes;
            var ec = projectile.GetComponent<EquipmentComponent>();
            ec.EquipWeapon(EquipmentComponent.CurrentWeapon);
            if (EquipmentComponent.CurrentWeapon.WeaponData.AppliesEffect)
            {
                var cpc = projectile.GetComponent<CollisionProjectileComponent>();
                cpc.AppliesEffect = true;
                var seac = projectile.GetComponent<StatusEffectAttributesComponent>();
                seac.StatusEffectAttributes = EquipmentComponent.CurrentWeapon.Effect;
            }

            //set animation
            float attackSpeed = AttributesComponent.AttackSpeed;
            SpriteComponent.Sprite.PlayAnimation(EquipmentComponent.CurrentWeapon.WeaponData.AnimationName + (MovementComponent.CurrentDirection == Direction.None ? "Down" : MovementComponent.CurrentDirection.ToString()), TimeSpan.FromMilliseconds(attackSpeed));
        }

        public void BeginBlock()
        {
            if ((DateTime.Now - _LastBlock).TotalMilliseconds > AttributesComponent.BlockSpeed)
            {
                _LastBlock = DateTime.Now;

                //Stop walking when we start to block.
                MovementComponent.StopWalking();

                IsBlocking = true;

                //TODO: Get a proper animation.
                var Animation = SpriteComponent.Sprite.Animations["Block" + MovementComponent.CurrentDirection.ToString()];
                if (SpriteComponent.Sprite.ActiveAnimation != Animation)
                    SpriteComponent.Sprite.PlayAnimation(Animation.Name);
            }
        }

        public void EndBlock()
        {
            IsBlocking = false;

            SpriteComponent.Sprite.PlayAnimation("Idle" + MovementComponent.CurrentDirection.ToString());
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
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            GameTime = Time;
            //TODO: Potential for bugs when we are combining both booleans.
            if (IsAttackingMelee || IsAttackingRanged)
            {
                _AttackTimer += Time.ElapsedGameTime;
                if (_AttackTimer >= TimeSpan.FromMilliseconds(AttributesComponent.AttackSpeed))
                {
                    IsAttackingMelee = false;
                    IsAttackingRanged = false;
                    _AttackTimer = TimeSpan.Zero;
                    
                }
            }

            //Basically, if the player is walking and blocking, do blocking, which stops walking.
            if (MovementComponent.IsWalking && IsBlocking)
            {
                BeginBlock();
            }
        }

        private Rectangle CreateHitBox()
        {
            //For each entity that is contained within our attack rectangle, and that isn't us, apply damage.
            //The attack rectange is calculated using our centre, range, and half our height.
            int MeleeAttackRange = (int)AttributesComponent.MeleeAttackRange.X; //This is the number we use to calculate our attack rectangle's start x position and width. So basically, it's horizontal range.
            int attackHeight = (int)AttributesComponent.MeleeAttackRange.Y; //This is the number we use to calculate our attack rectangle's start y position and height. So basically, it's vertical range.

            Rectangle attackRectangle;
            if (MovementComponent.CurrentDirection == Direction.Left)
                attackRectangle = new Rectangle(Parent.Location.Center.X - MeleeAttackRange, Parent.Location.Y - attackHeight, MeleeAttackRange, Parent.Location.Height + attackHeight);
            else if (MovementComponent.CurrentDirection == Direction.Right)
                attackRectangle = new Rectangle(Parent.Location.Center.X, Parent.Location.Y - attackHeight, MeleeAttackRange, Parent.Location.Height + attackHeight);
            else
            {
                //Player hasn't moved yet or just isn't facing left or right.
                //Set their rectangle to themselves (anything that is inside them will be attacked). Until we figure out how best to handle it anyways.
                attackRectangle = Parent.Location;
            }
            return attackRectangle;
        }

        private Entity CreateProjectileEntity(string name, Vector2 velocity)
        {
            var projectile = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(name).CreateEntity();
            projectile.Position = new Vector2(Parent.Location.Center.X, Parent.Location.Top);
            projectile.Size = new Vector2(12, 12); //TODO: Might need to track this as well.
            Parent.Scene.AddEntity(projectile);

            //Sets the entities this can attack.
            var cpc = projectile.GetComponent<CollisionProjectileComponent>();
            cpc.Classification = this.AttackableEntities;

            //Set projectile velocity.
            var pc = projectile.GetComponent<PhysicsComponent>();
            if (MovementComponent.CurrentDirection == Direction.Right)
                pc.Velocity = new Vector2(velocity.X, -velocity.Y);
            else if (MovementComponent.CurrentDirection == Direction.Left)
                pc.Velocity = new Vector2(-velocity.X, -velocity.Y);

            return projectile;
        }

        private EquipmentComponent EquipmentComponent;
        private AttributesComponent AttributesComponent;
        private MovementComponent MovementComponent;
        private SpriteComponent SpriteComponent;
        private PhysicsSystem PhysicsSystem;
        private PhysicsComponent PhysicsComponent;
	}
}