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
        /// Gets or sets a value indicating that this entity is attacking.
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
        private TimeSpan _Timer = new TimeSpan(); //not sure if this is the best way to set up attack speed.
        private EntityClassification _AttackableEntities;
        private bool _IsAttacking = false;
        private bool _IsBlocking = false;

        /// <summary>
        /// Attacks an enemy with a close range attack.
        /// </summary>
        ///<remarks>
        /// I'm assuming this is what the player uses. As in, the enemies will have their own melee attack function.
        /// I say this because I'm also assuming that EquipmentComponent is only used by players. Although, it would be
        /// easy (maybe) to create a generic melee attack function but it might seem messy.
        /// </remarks>
		public void AttackMelee() {
            //This guy is attacking, don't do anything.
            if (_IsAttacking)
                return;
            _IsAttacking = true;
            // TODO: Maybe the attack should start at a specific frame.
			// TODO: Limit number of attacks they can do.
			// TODO: Decide on how best to integrate things that are mutually exclusive, like attacking while walking.
			// At the very least the sprites for it will be mutually exclusive.
            float attackSpeed = AttributesComponent.AttackSpeed;
            SpriteComponent.Sprite.PlayAnimation(EquipmentComponent.CurrentWeapon.AnimationName + (MovementComponent.CurrentDirection == Direction.None ? "Down" : MovementComponent.CurrentDirection.ToString()), TimeSpan.FromMilliseconds(attackSpeed));
            
            //For each entity that is contained within our attack rectangle, and that isn't us, apply damage.
            //The attack rectange is calculated using our centre, range, and half our height.
            int MeleeAttackRange = (int)AttributesComponent.MeleeAttackRange.X; //This is the number we use to calculate our attack rectangle's start x position and width. So basically, it's horizontal range.
            int attackHeight = (int)AttributesComponent.MeleeAttackRange.Y; //This is the number we use to calculate our attack rectangle's start y position and height. So basically, it's vertical range.
            
            Rectangle attackRectangle;
            if (MovementComponent.CurrentDirection == Direction.Left)
                attackRectangle = new Rectangle(Parent.Location.Center.X - MeleeAttackRange, Parent.Location.Y - attackHeight, MeleeAttackRange, Parent.Location.Height + attackHeight);
            else if (MovementComponent.CurrentDirection == Direction.Right)
                attackRectangle = new Rectangle(Parent.Location.Center.X , Parent.Location.Y - attackHeight, MeleeAttackRange, Parent.Location.Height + attackHeight);
            else
            {
                //Player hasn't moved yet or just isn't facing left or right.
                //Set their rectangle to themselves (anything that is inside them will be attacked). Until we figure out how best to handle it anyways.
                attackRectangle = Parent.Location;
            }

            //Enumerate over each entity that intersected with our attack rectangle, and if they're not us, make them take damage.
            foreach (var attackedEntity in PhysicsSystem.GetEntitiesAtLocation(attackRectangle))
            {
                //might be a little inefficient because we have to keep searching a list to get the ClassificationComponent.
                var cc = attackedEntity.GetComponent<ClassificationComponent>(); 
                if (attackedEntity != Parent && cc.Classification == AttackableEntities)
                {
                    var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                    damageComponent.TakeDamage(AttributesComponent);
                }
            }
		}

        //TODO: Should possibly define a Component(?) that defines the projectile properties (things like GravityCoefficient, etc). 
        public void AttackRanged()
        {
            var projectile = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("TestProjectile").CreateEntity();

            var cc = projectile.GetComponent<CollisionDamageComponent>();
            cc.Damage = 50;

            var pc = projectile.GetComponent<PhysicsComponent>();
            pc.GravityCoefficient = 0.1f; //Some bullet drop, cause we're fancy.
            pc.HorDragCoefficient = 0;

            projectile.Size = new Vector2(12, 12);

            if (MovementComponent.CurrentDirection == Direction.Left)
            {
                projectile.Position = new Vector2(Parent.Location.Center.X, Parent.Location.Top);
                pc.VelocityX = -1000;  
            }
            else if (MovementComponent.CurrentDirection == Direction.Right)
            {
                projectile.Position = new Vector2(Parent.Location.Center.X, Parent.Location.Top);
                pc.VelocityX = 1000;
            }

            Parent.Scene.AddEntity(projectile);
        }

        public void BeginBlock()
        {
            //Stop walking when we start to block.
            MovementComponent.StopWalking();

            IsBlocking = true;
            
            //TODO: Get a proper animation.
            var Animation = SpriteComponent.Sprite.Animations["BoredDown"];
            if (SpriteComponent.Sprite.ActiveAnimation != Animation)
                SpriteComponent.Sprite.PlayAnimation(Animation.Name);
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
            //Seems messy doing it this way.
            if (_IsAttacking)
            {
                _Timer += Time.ElapsedGameTime;
                if (_Timer >= TimeSpan.FromMilliseconds(AttributesComponent.AttackSpeed))
                {
                    _IsAttacking = false;
                    _Timer = TimeSpan.Zero;
                }
            }

            //Basically, if the player is walking and blocking, do blocking, which stops walking.
            if (MovementComponent.IsWalking && IsBlocking)
            {
                BeginBlock();
            }
        }

        //The problem with this is that i'm assuming everything has an equipment component. So, if an enemy attacks, then they need the EquipmentComponent as well, which wouldnt make sense.
        //I'm also assuming that this is what enemies will use (by this, i mean CombatComponent) for handling their attacks.
        private EquipmentComponent EquipmentComponent;
        private AttributesComponent AttributesComponent;
        private MovementComponent MovementComponent;
        private SpriteComponent SpriteComponent;
        private PhysicsSystem PhysicsSystem;
        private PhysicsComponent PhysicsComponent;
	}
}