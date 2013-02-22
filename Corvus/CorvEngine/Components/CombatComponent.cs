using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace CorvEngine.Components
{
	public class CombatComponent : Component {

		// TODO: Add classification for what can be attacked.

		//This doesn't launch a projectile.
		//We simply get an x,y value to attack and get the entity there, in order to apply damage.
		public void AttackSword() {
			MovementComponent mc = Parent.GetComponent<MovementComponent>();
			SpriteComponent sc = Parent.GetComponent<SpriteComponent>();

			int attackRange = 50; //This is the number we use to calculate our attack point. Range.
			int attackPoint; //This eventually is calculated based on the range, and depends on what direction we're facing.

			if(mc.CurrentDirection == Direction.Left) {
				attackPoint = attackRange * -1;
			} else if(mc.CurrentDirection == Direction.Right) {
				attackPoint = attackRange;
			} else {//Player hasn't moved yet or just isn't facing left or right.
				attackPoint = 0;
			}

			// TODO: Provide an attack speed that makes them take that long to attack.
			// TODO: Limit number of attacks they can do.
			// TODO: Decide on how best to integrate things that are mutually exclusive, like attacking while walking.
			// At the very least the sprites for it will be mutually exclusive.
			sc.Sprite.PlayAnimation("Attack" + (mc.CurrentDirection == Direction.None ? "Down" : mc.CurrentDirection.ToString()), TimeSpan.FromMilliseconds(250));

			// TODO: This will make the user hit themselves potentially.
			// When switching to PhysicsSystem, make sure to specify classification.
			var attackedEntity = Scene.GetEntityAtPosition(new Point((int)Parent.Location.Center.X + attackPoint, Parent.Location.Y));

			if(attackedEntity != null) {
				var damageComponent = attackedEntity.GetComponent<DamageComponent>();
				damageComponent.TakeDamage(50);
			}
		}
	}
}
