using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities
{
    public class MovementComponent : Component
    {
        public float maxWalkVelocity = 500f;
        public float maxJumpVelocity = 1050f;
        public float gravity = 5000.5f;
        public bool isJumping = false;
        public bool isGrounded = true;
        public bool jumpStart = false; //This flag is just essentially to account for the fact that we're grounded on the first jump. Could maybe do something like airtime too eventually.
        public Direction CurrDir = Direction.Down;
    }
}
