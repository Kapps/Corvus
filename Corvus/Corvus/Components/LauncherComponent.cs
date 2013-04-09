using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay;

namespace Corvus.Components
{
    //Could use knockback collision component but i messed that up by making it X only. Changing it would be a pain.
    /// <summary>
    /// A component to launch entities.   
    /// </summary>
    public class LauncherComponent : CollisionEventComponent
    {
        private Vector2 _LaunchVelocity = new Vector2();
        private string _LaunchSound = "Explode4";

        /// <summary>
        /// Gets or sets the launch velocity.
        /// </summary>
        public Vector2 LaunchVelocity
        {
            get { return _LaunchVelocity; }
            set { _LaunchVelocity = value; }
        }

        /// <summary>
        /// Gets or sets the sound to play when an entity is launched.
        /// </summary>
        public string LaunchSound
        {
            get { return _LaunchSound; }
            set { _LaunchSound = value; }
        }

        protected override bool OnCollision(Entity Entity, EntityClassification Classification)
        {
            var pc = Entity.GetComponent<PhysicsComponent>();
            if (pc == null)
                return false;
            CorvEngine.AudioManager.PlaySoundEffect(LaunchSound);
            pc.Velocity = LaunchVelocity;
            return true;   
        }
    }
}
