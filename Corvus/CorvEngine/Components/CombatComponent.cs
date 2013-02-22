using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace CorvEngine.Components
{
    public class CombatComponent : Component
    {
        //This doesn't launch a projectile.
        //We simply get an x,y value to attack and get the entity there, in order to apply damage.
        public void AttackSword()
        {
            MovementComponent mc = Parent.GetComponent<MovementComponent>();

            int attackRange = 50; //This is the number we use to calculate our attack point. Range.
            int attackPoint; //This eventually is calculated based on the range, and depends on what direction we're facing.

            if (mc.LastWalkDir == Direction.Left)
            {
                attackPoint = attackRange * -1;
            }
            else if (mc.LastWalkDir == Direction.Right)
            {
                attackPoint = attackRange;
            }
            else //Player hasn't moved yet or just isn't facing left or right.
            {
                attackPoint = 0;
            }

            var attackedEntity = Scene.GetEntityAtPosition(new Point((int)Parent.Location.Center.X + attackPoint, Parent.Location.Y));

            if (attackedEntity != null)
            {
                var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                damageComponent.TakeDamage(50);
            }
        }
    }
}
