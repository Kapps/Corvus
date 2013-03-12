using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;

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
	[Serializable]
	public abstract class CollisionEventComponent : Component {
		private DateTime _LastTriggered;
		private TimeSpan _MinimumTriggerDelay;

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

		/// <summary>
		/// Gets or sets the minimum delay between triggers of this component.
		/// That is, this component will not be triggered more often than this value.
		/// This property is ignored when DisposeOnCollision is true.
		/// The default value is zero, or no delay.
		/// Note that this applies to all components triggering this one, not on a per-component basis.
		/// </summary>
		public TimeSpan MinimumTriggerDelay {
			get { return _MinimumTriggerDelay; }
			set { _MinimumTriggerDelay = value; }
		}

		/// <summary>
		/// Gets the time that this component was last triggered, or null if it has not been triggered yet.
		/// </summary>
		public DateTime? LastTriggered {
			get { return _LastTriggered; }
		}

		protected override void OnInitialize() {
			base.OnInitialize();
			var Physics = GetDependency<PhysicsComponent>();
			Physics.Collided += Physics_Collided;
		}

		void Physics_Collided(PhysicsComponent Component, PhysicsComponent Other) {
			CollisionDetected(Component, Other, true);
		}

		private void CollisionDetected(PhysicsComponent Component, PhysicsComponent Other, bool TriggerRemaining) {
			if(this.IsDisposed) // Prevent firing multiple times.
				return;
			if(this.LastTriggered.HasValue && (DateTime.Now - this.LastTriggered.Value) < MinimumTriggerDelay)
				return; // Limit to MinimumTriggerDelay since the last event.
			var Classification = Other.Parent.GetComponent<ClassificationComponent>();
			if(_Classification != EntityClassification.Any && (Classification == null || (Classification.Classification & this.Classification) == 0))
				return;
			bool Valid = OnCollision(Other.Parent, Classification == null ? EntityClassification.Unknown : Classification.Classification);
			if(Valid) {
				this._LastTriggered = DateTime.Now;
				if(DisposeOnCollision) {
					this.Dispose();
					if(TriggerRemaining) {
						// TODO: This should only trigger things not already triggered for this collision.
						// (Aka, DisposeOnCollision is false and already triggered before this one).
						foreach(var OtherEvent in Parent.Components.Select(c => c as CollisionEventComponent).Where(c => c != null && c != this).ToArray())
							OtherEvent.CollisionDetected(Component, Other, false);
						Parent.Dispose();
					}
				}
			}
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
