using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace CorvEngine.Components {
	/// <summary>
	/// Implements a Component used to provide movement and jumping capabilites to an Entity.
	/// </summary>
	[Serializable]
	public class MovementComponent : Component {
		private PhysicsComponent PhysicsComponent;
		private SpriteComponent SpriteComponent;
		private float _WalkSpeed = 500;
        private float _WalkSpeedModifier = 1f;
        private float _CombatWalkSpeedModifier = 1f;
		private float _JumpSpeed = 950;
        private float _JumpSpeedModifier = 1f;
		private float _WalkAcceleration = 15000;
		private Direction _CurrentDirection = Direction.Down;
		private Direction _WalkDirection = Direction.None;
        private bool _IsWalking = false;

        /// <summary>
        /// Gets or sets the maximum speed that this Entity can walk at multiplied by the walk speed modifier.
        /// </summary>
		public float MaxWalkingSpeed {
			get { return WalkSpeed * WalkSpeedModifier * CombatWalkSpeedModifier; }
		}

        /// <summary>
        /// Gets or sets how fast this Entity jumps multiplied by the jump speed modifier.
        /// Note that this does not take into consideration gravity.
        /// </summary>
        public float MaxJumpSpeed {
            get { return JumpSpeed * JumpSpeedModifier; }
        }

        /// <summary>
        /// Gets or sets the walking speed that this Entity can walk at, in units per second.
        /// </summary>
        public float WalkSpeed
        {
            get { return _WalkSpeed; }
            set { _WalkSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the walk speed modifier.
        /// </summary>
        public float WalkSpeedModifier
        {
            get { return _WalkSpeedModifier; }
            set { _WalkSpeedModifier = value; }
        }

        /// <summary>
        /// Gets or sets a combat walks speed modifier. 
        /// Basically a hack so I use speed modifier status effects without screwing up how attacking works.
        /// </summary>
        public float CombatWalkSpeedModifier
        {
            get { return _CombatWalkSpeedModifier; }
            set { _CombatWalkSpeedModifier = value; }
        }

		/// <summary>
		/// Gets or sets the speed to increase velocity by each frame when walking, up to MaxWalkingSpeed.
		/// This value is in units per second, and is affected by HorizontalDrag.
		/// </summary>
		public float WalkAcceleration {
			get { return _WalkAcceleration; }
			set { _WalkAcceleration = value; }
		}

		/// <summary>
		/// Gets or sets how fast this Entity jumps, in units per second.
		/// Note that this does not take into consideration gravity.
		/// </summary>
		public float JumpSpeed {
			get { return _JumpSpeed; }
			set { _JumpSpeed = value; }
		}

        /// <summary>
        /// Gets or sets how much to modifier the jump speed.
        /// </summary>
        public float JumpSpeedModifier
        {
            get { return _JumpSpeedModifier; }
            set { _JumpSpeedModifier = value; }
        }

		/// <summary>
		/// Gets or sets the direction that this Entity is facing.
		/// </summary>
		public Direction CurrentDirection {
			get { return _CurrentDirection; }
			set { _CurrentDirection = value; }
		}

        /// <summary>
        /// Gets or sets a value  that indicates whether this entity is walking.
        /// </summary>
        public bool IsWalking {
            get { return _IsWalking; }
            set { _IsWalking = value; }
        }

        /// <summary>
        /// Gets or sets the walk direction.
        /// </summary>
        public Direction WalkDirection
        {
            get { return _WalkDirection; }
            set { _WalkDirection = value; }
        }

		/// <summary>
		/// Causes this Entity to being walking in the given direction, either Left or Right.
		/// Walking may then be stopped through the StopWalking method.
		/// </summary>
		public void BeginWalking(Direction dir) {
			if(dir != Direction.Left && dir != Direction.Right)
				throw new ArgumentException("Walk only applies to the Left and Right directions.");

			_WalkDirection = dir;
			CurrentDirection = _WalkDirection;
            IsWalking = true;
			var Animation = SpriteComponent.Sprite.Animations["Walk" + CurrentDirection.ToString()];
			if(SpriteComponent.Sprite.ActiveAnimation != Animation)
				SpriteComponent.Sprite.PlayAnimation(Animation.Name);
		}

		/// <summary>
		/// Informs the Entity to stop walking, no longer applying walk velocity.
		/// If the Entity is not walking, this method does nothing.
		/// </summary>
		public void StopWalking() {
            _WalkDirection = Direction.None; 
            IsWalking = false;
			SpriteComponent.Sprite.PlayAnimation("Idle" + CurrentDirection.ToString());
		}

		/// <summary>
		/// Start a jump. Sets necessary flags and adjusts Y velocity for a jump.
		/// </summary>
		public void Jump(bool allowMulti) {
			if(PhysicsComponent.IsGrounded || allowMulti)
				PhysicsComponent.VelocityY = -MaxJumpSpeed;
		}

        /// <summary>
        /// Knocksback this entity by an approx. distance.  
        /// </summary>
        public void Knockback(float distance, int direction)
        {
            //TODO: find an actually formula for this.
            float t = 0.100f;//s
            float vel = distance / t;
            /// PhysicsComponent.VelocityY += -500;
            PhysicsComponent.VelocityX += direction * vel;// direction* vel; //new Vector2(direction * vel, -1 * 2000);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            PhysicsComponent = GetDependency<PhysicsComponent>();
            SpriteComponent = GetDependency<SpriteComponent>();
        }

		protected override void OnUpdate(GameTime Time) {
            base.OnUpdate(Time);
            if (_WalkDirection == Direction.Left)
                PhysicsComponent.VelocityX -= Math.Max(0, Math.Min(MaxWalkingSpeed + PhysicsComponent.VelocityX, WalkAcceleration * Time.GetTimeScalar()));
            else if (_WalkDirection == Direction.Right)
                PhysicsComponent.VelocityX += Math.Max(0, Math.Min(MaxWalkingSpeed - PhysicsComponent.VelocityX, WalkAcceleration * Time.GetTimeScalar()));
           
        }
	}
}
