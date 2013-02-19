﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;
using Microsoft.Xna.Framework;

namespace CorvEngine.Components {

	/// <summary>
	/// A delegate used when a component between two systems occurs.
	/// </summary>
	public delegate void CollisionDelegate(PhysicsComponent Component, PhysicsComponent Other);

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
		/// Indicates whether this Entity should collide with static objects, such as tiles.
		/// </summary>
		public bool PerformStaticCollision {
			get { return _PerformStaticCollision; }
			set { _PerformStaticCollision = value; }
		}

		/// <summary>
		/// Indicates whether this Entity should collide with dynamic objects, such as other Entities.
		/// </summary>
		public bool PerformDynamicCollision {
			get { return _PerformDynamicCollision; }
			set { _PerformDynamicCollision = value; }
		}

		/// <summary>
		/// Gets or sets the amount to multiply the force of gravity by.
		/// For an Entity that should not be affected by gravity, this should be 0.
		/// </summary>
		public float GravityCoefficient {
			get { return _GravityCoefficient; }
			set { _GravityCoefficient = value; }
		}

		/// <summary>
		/// Notifies this PhysicsComponent that a collision occurred with the other component.
		/// </summary>
		public void NotifyCollision(PhysicsComponent Other) {

		}

		private float _GravityCoefficient = 1;
		private Vector2 _Velocity;
		private bool _IsGrounded = false;
		private bool _PerformStaticCollision = true;
		private bool _PerformDynamicCollision = true;
	}
}
