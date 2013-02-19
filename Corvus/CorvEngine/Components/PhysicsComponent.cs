using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;
using Microsoft.Xna.Framework;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides a component used to give components physics information and cause the PhysicsSystem to use it.
	/// </summary>
	public class PhysicsComponent : Component {

		/// <summary>
		/// Gets the velocity of this entity.
		/// </summary>
		public Vector2 Velocity {
			get { return _Velocity; }
			set { _Velocity = value; }
		}

		/// <summary>
		/// Gets or sets the X component of the velocity.
		/// This is simply a shortcut to work around property limitations.
		/// </summary>
		public float VelocityX {
			get { return _Velocity.X; }
			set { Velocity = new Vector2(value, Velocity.Y); }
		}

		/// <summary>
		/// Gets or sets the Y component of the velocity.
		/// This is simply a shortcut to work around property limitations.
		/// </summary>
		public float VelY {
			get { return _Velocity.Y; }
			set { Velocity = new Vector2(Velocity.X, value); }
		}

		/// <summary>
		/// Indicates if this Entity is currently touching the ground.
		/// This is generally assigned by a PhysicsSystem.
		/// </summary>
		public bool IsGrounded {
			get { return _IsGrounded; }
			set { _IsGrounded = value; }
		}

		/// <summary>
		/// Gets or sets the amount to multiply the force of gravity by.
		/// For an Entity that should not be affected by gravity, this should be 0.
		/// </summary>
		public float GravityCoefficient {
			get { return _GravityCoefficient; }
			set { _GravityCoefficient = value; }
		}

		private float _GravityCoefficient = 1;
		private Vector2 _Velocity;
		private bool _IsGrounded = false;
	}
}
