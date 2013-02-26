using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine;

namespace Corvus.Components {
	public class CombatComponent : Component {
        //TODO: Maybe add attack speed so weapons affect them. 
        //TODO: Same as above but for range.

		// TODO: Add classification for what can be attacked.

        public string AttackAnimation
        {
            get { return _AttackAnimation; }
            set { _AttackAnimation = value; }
        }

        private string _AttackAnimation;

		//This doesn't launch a projectile.
		//We simply get an x,y value to attack and get the entity there, in order to apply damage.
		public void AttackSword() {
			var mc = Parent.GetComponent<MovementComponent>();
			var sc = Parent.GetComponent<SpriteComponent>();
            var ps = Parent.Scene.GetSystem<PhysicsSystem>();
            var ac = this.GetDependency<AttributesComponent>();
            int attackRange = 100; //This is the number we use to calculate our attack rectangle's start x position and width. So basically, it's horizontal range.
            int attackHeight = 100; //This is the number we use to calculate our attack rectangle's start y position and height. So basically, it's vertical range.

			// TODO: Provide an attack speed that makes them take that long to attack.
			// TODO: Limit number of attacks they can do.
			// TODO: Decide on how best to integrate things that are mutually exclusive, like attacking while walking.
			// At the very least the sprites for it will be mutually exclusive.
			sc.Sprite.PlayAnimation(AttackAnimation + (mc.CurrentDirection == Direction.None ? "Down" : mc.CurrentDirection.ToString()), TimeSpan.FromMilliseconds(1000));

            //For each entity that is contained within our attack rectangle, and that isn't us, apply damage.
            //The attack rectange is calculated using our centre, range, and half our height.
            Rectangle attackRectangle;

            if (mc.CurrentDirection == Direction.Left)
            {
                attackRectangle = new Rectangle(Parent.Location.Center.X - attackRange, Parent.Location.Y - attackHeight, attackRange, Parent.Location.Height + attackHeight);
            }
            else if (mc.CurrentDirection == Direction.Right)
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
            foreach (var attackedEntity in ps.GetEntitiesAtLocation(attackRectangle))
            {
                if (attackedEntity != Parent)
                {
                    var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                    damageComponent.TakeDamage(ac);
                }
            }
		}

        public void AttackGun()
        {

        }
	}
}