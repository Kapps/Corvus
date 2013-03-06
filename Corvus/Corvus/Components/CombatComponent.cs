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

        private EntityClassification _AttackableEntities;
        //The problem with this is that i'm assuming everything has an equipment component. So, if an enemy attacks, then they need the EquipmentComponent as well, which wouldnt make sense.
        //I'm also assuming that this is what enemies will use (by this, i mean CombatComponent) for handling their attacks.
        private EquipmentComponent EquipmentComponent;
        private AttributesComponent AttributesComponent;
        private MovementComponent MovementComponent;
        private SpriteComponent SpriteComponent;
        private PhysicsSystem PhysicsSystem;
        private PhysicsComponent PhysicsComponent;
        private TimeSpan _Timer = new TimeSpan(); //not sure if this is the best way to set up attack speed.
        private bool _IsAttacking = false;
        private bool _IsBlocking = false;

        /// <summary>
        /// Gets or sets a value indicating that this entitiy is blocking.
        /// </summary>
        public bool IsBlocking
        {
            get { return _IsBlocking; }
            set { _IsBlocking = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this entity is attacking.
        /// </summary>
        public bool IsAttacking
        {
            get { return _IsAttacking; }
            set { _IsAttacking = value; }
        }

		//This doesn't launch a projectile.
		//We simply get an x,y value to attack and get the entity there, in order to apply damage.
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
            int attackRange = (int)AttributesComponent.AttackRange.X; //This is the number we use to calculate our attack rectangle's start x position and width. So basically, it's horizontal range.
            int attackHeight = (int)AttributesComponent.AttackRange.Y; //This is the number we use to calculate our attack rectangle's start y position and height. So basically, it's vertical range.
            Rectangle attackRectangle;
            if (MovementComponent.CurrentDirection == Direction.Left)
            {
                attackRectangle = new Rectangle(Parent.Location.Center.X - attackRange, Parent.Location.Y - attackHeight, attackRange, Parent.Location.Height + attackHeight);
            }
            else if (MovementComponent.CurrentDirection == Direction.Right)
            {
                attackRectangle = new Rectangle(Parent.Location.Center.X, Parent.Location.Y - attackHeight, attackRange, Parent.Location.Height + attackHeight);
            }
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

        public void AttackRanged()
        {
            Entity projectile = new Entity();
            //Scene.AddEntity(TestGames);
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
            //Seems messy doing it his way.
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
	}
}