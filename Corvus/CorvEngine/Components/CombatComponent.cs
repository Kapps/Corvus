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
        public void AttackSword()
        {
            MovementComponent mc = Parent.GetComponent<MovementComponent>();

            int attackRange = 50;
            int attackPoint;

            if (mc.LastWalkDir == Direction.Left)
            {
                attackPoint = attackRange * -1;
            }
            else if (mc.LastWalkDir == Direction.Right)
            {
                attackPoint = attackRange;
            }
            else
            {
                attackPoint = 0;
            }

            var attackedEntity = Scene.GetEntityAtPosition(new Point((int)Parent.CentreX + attackPoint, Parent.Location.Y));

            if (attackedEntity != null)
            {
                var damageComponent = attackedEntity.GetComponent<DamageComponent>();
                damageComponent.TakeDamage(50);
            }
        }
    }
}
