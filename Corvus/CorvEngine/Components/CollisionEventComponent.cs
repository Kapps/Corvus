using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;

namespace CorvEngine.Components {

	/// <summary>
	/// Provides EventArgs for when a collision that triggers a CollisionEventComponent occurs.
	/// </summary>
	public class CollisionEventArgs : EventArgs {

		/// <summary>
		/// Indicates whether the collision event was cancelled, preventing disposing if DisposeOnCollision is true.
		/// </summary>
		public bool Cancelled {
			get { return _Cancelled; }
			set { _Cancelled = value; }
		}

		/// <summary>
		/// Gets the Entity that was collided with.
		/// </summary>
		public Entity Entity {
			get { return _Entity; }
		}

		public CollisionEventArgs(Entity Entity) {
			this._Entity = Entity;
		}

		private bool _Cancelled = false;
		private Entity _Entity;
	}

	/// <summary>
	/// Provides the base class for a Component that handles collisions with a given Entity, restricted to a specific classification.
	/// </summary>
	public abstract class CollisionEventComponent : Component {

		/// <summary>
		/// Gets or sets the classification that the Entity needs to have for the damage to occur.
		/// The default is to affect all entities.
		/// </summary>
		public EntityClassification Classification {
			get { return _Classification; }
			set { _Classification = value; }
		}

		/// <summary>
		/// Indicates whether this component should be disposed of after a collision is handled.
		/// </summary>
		public bool DisposeOnCollision {
			get { return _DisposeOnCollision; }
			set { _DisposeOnCollision = value; }
		}

		protected override void OnInitialize() {
			base.OnInitialize();
			var Physics = GetDependency<PhysicsComponent>();
			Physics.Collided += Physics_Collided;
		}

		void Physics_Collided(PhysicsComponent Component, PhysicsComponent Other) {
			if(this.IsDisposed)
				return;
			var Classification = Other.Parent.GetComponent<ClassificationComponent>();
			if(_Classification != EntityClassification.Any && (Classification == null || (Classification.Classification & this.Classification) == 0))
				return;
			bool Valid = OnCollision(Other.Parent, Classification == null ? EntityClassification.Unknown : Classification.Classification);
			if(Valid && DisposeOnCollision)
				this.Parent.Dispose();
		}

		/// <summary>
		/// Called when a collision with the specified Entity of the given Classification occurs.
		/// Returns whether or not the collision was valid, causing this object to be disposed if DisposeOnCollision was true.
		/// </summary>
		protected abstract bool OnCollision(Entity Entity, EntityClassification Classification);

		private EntityClassification _Classification = EntityClassification.Any;
		private bool _DisposeOnCollision = false;
	}
}
