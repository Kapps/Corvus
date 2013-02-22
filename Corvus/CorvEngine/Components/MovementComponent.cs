﻿using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

namespace CorvEngine.Components {
	public class MovementComponent : Component {
		float maxWalkVelocity = 500f;
		float maxJumpVelocity = 1050f;
		Direction CurrDir = Direction.None;
        public Direction LastWalkDir = Direction.None;
		PhysicsComponent Physics;

		protected override void OnInitialize() {
			base.OnInitialize();
			Physics = GetDependency<PhysicsComponent>();
		}

		/// <summary>
		/// Walk in a certain direction. Handles animation. Needs to be called each update.
		/// </summary>
		/// <param name="dir"></param>
		public void Walk(Direction dir) {
			// TODO: These should not just alter Velocity!
			switch(dir) {
				case Direction.Left:
					Physics.VelocityX = maxWalkVelocity * -1;
					if(CurrDir != dir)
						Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + dir.ToString());
                    LastWalkDir = Direction.Left;
					break;
				case Direction.Right:
					Physics.VelocityX = maxWalkVelocity;
					if(CurrDir != dir)
						Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Walk" + dir.ToString());
                    LastWalkDir = Direction.Right;
					break;
				case Direction.None:
					if(CurrDir != dir)
						Parent.GetComponent<SpriteComponent>().Sprite.PlayAnimation("Idle" + CurrDir);
					Physics.VelocityX = 0;
					break;
			}

			CurrDir = dir;
		}

		/// <summary>
		/// Start a jump. Sets necessary flags and adjusts Y velocity for a jump.
		/// </summary>
		/// <param name="allowMulti"></param>
		public void Jump(bool allowMulti) {
			if(Physics.IsGrounded || allowMulti) {
				Physics.VelY = maxJumpVelocity * -1 + 50;
			}
		}
	}
}
