using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides a System used to manage physics for Entities that have a PhysicsComponent.
	/// </summary>
	[Serializable]
	public class PhysicsSystem : SceneSystem {

		/// <summary>
		/// Indicates the force of gravity.
		/// This number is currently fairly arbitrary, and should be tweaked as appropriate.
		/// </summary>
		public float Gravity {
			get { return _Gravity; }
			set { _Gravity = value; }
		}

		/// <summary>
		/// Indicates how much to slow each Entity down in the horizontal direction each frame.
		/// This is essentially the force of Gravity, but horizontally, and applies both when grounded and when not.
		/// </summary>
		public float HorizontalDrag {
			get { return _HorizontalDrag; }
			set { _HorizontalDrag = value; }
		}

		/// <summary>
		/// Indicates the maximum amount of time that can pass in a single physics step.
		/// Updates where a longer period of time elapses are broken into multiple steps.
		/// </summary>
		public TimeSpan MaxStep {
			get { return _MaxStep; }
			set { _MaxStep = value; }
		}

		/// <summary>
		/// Indicates if a solid tile exists at the given location, or if that location is beneath the map and thus considered solid as well.
		/// </summary>
		public bool IsLocationSolid(Vector2 Location) {
			foreach(var Layer in Scene.Layers) {
				if(Layer.IsSolid && Layer.GetTileAtPosition(Location) != null)
					return true;
			}
			if(Location.Y >= Scene.MapSize.Y - Scene.TileSize.Y)
				return true;
			return false;
		}

		/// <summary>
		/// Returns all Entities that overlap the specified location.
		/// </summary>
		public IEnumerable<Entity> GetEntitiesAtLocation(Rectangle Location) {
			// TODO: Use tiles instead of this O(N) approach.
			return Scene.Entities.Where(c => c.Location.Intersects(Location));
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time) {
			PerformDynamicCollision(Time);
			PerformStaticCollision(Time);
		}

		private void PerformStaticCollision(GameTime Time) {
			// This is safe to multi-thread because we're never modifying anything besides the Component itself.
			// But each iteration is so cheap that the overhead of threading outweighs performance benefits.
			List<Task> Tasks = new List<Task>();
			foreach(var Component in GetFilteredComponents<PhysicsComponent>()) {
				var Parent = Component.Parent;
				if(!Component.IsGrounded)
                    Component.VelocityY += Gravity * Time.GetTimeScalar() * Parent.GetComponent<PhysicsComponent>().GravityCoefficient; //Prolly not the best thing to do GetComponent, eventually make something else.
				//if(Component.IsMoving) {
					float CurrSign = Math.Sign(Component.VelocityX);
					Component.VelocityX += -CurrSign * HorizontalDrag * Time.GetTimeScalar() * Parent.GetComponent<PhysicsComponent>().HorizontalDragCoefficient;
					if(Math.Sign(Component.VelocityX) != CurrSign)
						Component.VelocityX = 0;
				//}
				Vector2 PositionDelta = Component.Velocity * Time.GetTimeScalar();
				// First, handle falling. Check for collision a bit below bottom + PositionDelta, and put them back to the top of the tile if hit.
				if(Component.VelocityY >= 0 && CheckStaticCollision(Parent, new Vector2(0, (Parent.Size.Y / 2) + PositionDelta.Y + 1), (Tile) => new Vector2(Parent.Position.X, Tile.Location.Top - Parent.Size.Y))) {
					// We're going to hit a tile while falling, so stop falling.
					Component.IsGrounded = true;
					Component.VelocityY = 0;
					PositionDelta.Y = 0;
				} else {
					// Otherwise, we're not grounded, and check for collision above.
					Component.IsGrounded = false;
					if(Component.VelocityY < 0 && CheckStaticCollision(Parent, new Vector2(0, (-Parent.Size.Y / 2) + PositionDelta.Y - 1), (Tile) => new Vector2(Parent.Position.X, Tile.Location.Bottom))) {
						Component.VelocityY = 0;
						PositionDelta.Y = 0;
					}
				}
				// Now, check horizontal collision.
				if(Component.VelocityX >= 0.001f && CheckStaticCollision(Parent, new Vector2(Parent.Size.X / 2 + PositionDelta.X + 1, 0), (Tile) => new Vector2(Tile.Location.Left - Parent.Size.X, Parent.Position.Y))) {
					PositionDelta.X = 0;
					Component.VelocityX = 0;
				} else if(Component.VelocityX <= 0.001f && CheckStaticCollision(Parent, new Vector2(-Parent.Size.X / 2 + PositionDelta.X - 1, 0), (Tile) => new Vector2(Tile.Location.Right, Parent.Position.Y))) {
					PositionDelta.X = 0;
					Component.VelocityX = 0;
				}

				Parent.Position += PositionDelta;
				Parent.Y = Math.Max(0, Parent.Y);
				Parent.X = Math.Max(-Scene.TileSize.X / 2, Parent.X);
				Parent.X = Math.Min(Scene.MapSize.X - Scene.TileSize.X, Parent.X);
				// Special case: If we're at the bottom of the level, we should be considered grounded.
				if(Parent.Y > Scene.MapSize.Y - Parent.Size.Y) {
					Parent.Y = Scene.MapSize.Y - Parent.Size.Y;
					Component.VelocityY = 0;
					Component.IsGrounded = true;
				}
			}
		}

		private Tile GetSolidTile(Entity Entity, Vector2 Offset) {
			Vector2 OffsetLocation = Entity.Position + (Entity.Size / 2) + Offset;
			foreach(var Layer in Scene.Layers.Where(c => c.IsSolid)) {
				Tile Tile = Layer.GetTileAtPosition(OffsetLocation);
				if(Tile != null)
					return Tile;
			}
			return null;
		}

		private bool CheckStaticCollision(Entity Entity, Vector2 Offset, Func<Tile, Vector2> LocationAdjuster) {
			var Tile = GetSolidTile(Entity, Offset);
			if(Tile != null) {
				var AdjustedPosition = LocationAdjuster(Tile);
				Entity.Position = AdjustedPosition;
				return true;
			}
			return false;
		}

		private void PerformDynamicCollision(GameTime Time) {
			// This is safe to multi-thread since we're only marking what collided, not resolving the collision.
			// Instead of using Tasks and waiting on them though, we're just keeping track of how many objects we've checked collision for.
			// Since we know ahead of time how many there are to do it for, this provides us an efficient way to keep tarck.
			// Also, using a ConcurrentStack because it's likely to be the most efficient since it can just use a CAS instead of a lock.
			// Currently, this method creates a whole lot of garbage. Probably doesn't hurt too badly though.
			ConcurrentStack<CollisionInfo> Collisions = new ConcurrentStack<CollisionInfo>();
			var AllComponents = GetFilteredComponents<PhysicsComponent>().ToArray();
			int TasksComplete = 0;
			foreach(var First in AllComponents) {
				//ThreadPool.QueueUserWorkItem((unused) => {
					// Foreach loop variable will change to new instance; store the reference here instead.
					var Original = First;
					// TODO: Don't use this O(N^2) approach. Consider using a QuadTree, or just making use of Tiles.
					// For now though, we don't even have more than 20 entities per map, so...
					// Especially with multi-threading, this should be fast enough.
					foreach(var Second in AllComponents) {
						if(Second == Original)
							continue;
						if(Original.Parent.Location.Intersects(Second.Parent.Location)) {
							Collisions.Push(new CollisionInfo() {
								First = Original,
								Second = Second
							});
						}
					}
					Interlocked.Increment(ref TasksComplete);
				//});
			}

			HashSet<CollisionInfo> CurrentCollisions = new HashSet<CollisionInfo>();
			while(true) {
				bool FinishedAllTasks = TasksComplete == AllComponents.Length;
				CollisionInfo Collision;
				while(Collisions.TryPop(out Collision)) {
					if(!PreviousCollisions.Contains(Collision)) {
						if(!Collision.First.IsDisposed && !Collision.Second.IsDisposed) // A collision can dispose other components after all.
							Collision.First.NotifyCollision(Collision.Second);
					}
					CurrentCollisions.Add(Collision);
				}
				if(FinishedAllTasks)
					break;
			}
			PreviousCollisions = CurrentCollisions;
		}

		protected override void OnDraw() {
		}

		private float _Gravity = 4000;
		private float _HorizontalDrag = 12000;
		private TimeSpan _MaxStep = TimeSpan.FromMilliseconds(20);
		private HashSet<CollisionInfo> PreviousCollisions = new HashSet<CollisionInfo>();

		private struct CollisionInfo {
			public PhysicsComponent First;
			public PhysicsComponent Second;

			public override int GetHashCode() {
				return First.GetHashCode() ^ Second.GetHashCode();
			}

			public override bool Equals(object obj) {
				CollisionInfo ci = (CollisionInfo)obj;
				return ci.First == First && ci.Second == Second;
			}
		}
	}
}
