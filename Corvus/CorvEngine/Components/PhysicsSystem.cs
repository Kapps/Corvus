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

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time) {
			const float FRAME_SKIP_DURATION = 2000;
			// Do physics in steps, never doing more than a certain amount of time per step.
			// If it took more than 2 seconds since the last update, just skip this frame.
			if(Time.ElapsedGameTime.TotalMilliseconds > FRAME_SKIP_DURATION)
				return;
			TimeSpan Remaining = Time.ElapsedGameTime;
			while(Remaining > TimeSpan.Zero) {

				TimeSpan Delta = TimeSpan.FromTicks(Math.Min(MaxStep.Ticks, Remaining.Ticks));
				GameTime StepTime = new GameTime(Time.TotalGameTime.Subtract(Remaining), Delta);
				Remaining = Remaining.Subtract(Delta);

				PerformDynamicCollision(StepTime);
				PerformStaticCollision(StepTime);
			}
		}

		private void PerformStaticCollision(GameTime Time) {
			// This is safe to multi-thread because we're never modifying anything besides the Component itself.
			List<Task> Tasks = new List<Task>();
			foreach(var Component in GetFilteredComponents<PhysicsComponent>()) {
				bool AnySolidHit = false;
				var Parent = Component.Parent;
				if(!Component.IsGrounded)
					Component.VelocityY += Gravity * Time.GetTimeScalar();
				if(Component.IsMoving) {
					float CurrSign = Math.Sign(Component.VelocityX);
					Component.VelocityX += -CurrSign * HorizontalDrag * Time.GetTimeScalar();
					if(Math.Sign(Component.VelocityX) != CurrSign)
						Component.VelocityX = 0;
				}
				Vector2 PositionDelta = Component.Velocity * Time.GetTimeScalar();
				foreach(var Layer in Scene.Layers.Where(c => c.IsSolid)) {
					Tile Tile = Layer.GetTileAtPosition(Parent.Position + new Vector2((Parent.Size / 2).X, Parent.Size.Y));
					if(Tile != null && Layer.IsSolid && Math.Abs(Parent.Location.Bottom - Tile.Location.Top) < Math.Abs(PositionDelta.Y) + 1.1f) {
						var TileAbove = Layer.GetTile((int)Tile.TileCoordinates.X, (int)Tile.TileCoordinates.Y - 1);
						if(TileAbove != null)
							continue; // Don't detect this as being hitting the floor because we're inside a spot that's solid wall.
						if(Component.VelocityY < 0) // Don't 'fall' on to the tile if we're still going up.
							continue;
						Parent.Y = Tile.Location.Top - Tile.Location.Height;
						AnySolidHit = true;
					}
				}

				if(AnySolidHit) { //If hitting any solid object OR not hitting any tile, ground us.
					Component.IsGrounded = true;
					Component.VelocityY = 0;
					PositionDelta.Y = 0;
				} else { //We're not yet on ground.
					Component.IsGrounded = false;
				}

				Parent.Position += PositionDelta;
				Parent.Y = Math.Max(0, Parent.Y);
				Parent.X = Math.Max(-Scene.TileSize.X / 2, Parent.X);
				Parent.X = Math.Min(Scene.MapSize.X - Scene.TileSize.X, Parent.X);
				// Special case: If we're at the bottom of the level, we should be considered grounded.
				if(Parent.Y > Scene.MapSize.Y - Scene.TileSize.Y) {
					Parent.Y = Scene.MapSize.Y - Scene.TileSize.Y;
					Component.VelocityY = 0;
					Component.IsGrounded = true;
				}
			}
		}

		private void PerformDynamicCollision(GameTime Time) {
//#if DEBUG
			//var DuplicateCount = GetFilteredComponents<PhysicsComponent>().Select(c => GetFilteredComponents<PhysicsComponent>().Where(d => d == c).Count()).ToArray();
			//Console.WriteLine(DuplicateCount);
//#endif
			// This is safe to multi-thread since we're only marking what collided, not resolving the collision.
			// Instead of using Tasks and waiting on them though, we're just keeping track of how many objects we've checked collision for.
			// Since we know ahead of time how many there are to do it for, this provides us an efficient way to keep tarck.
			// Also, using a ConcurrentStack because it's likely to be the most efficient since it can just use a CAS instead of a lock.
			ConcurrentStack<CollisionInfo> Collisions = new ConcurrentStack<CollisionInfo>();
			var AllComponents = GetFilteredComponents<PhysicsComponent>().ToArray();
			int TasksComplete = 0;
			foreach(var First in AllComponents) {
				ThreadPool.QueueUserWorkItem((unused) => {
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
				});
			}

			HashSet<CollisionInfo> CurrentCollisions = new HashSet<CollisionInfo>();
			while(true) {
				bool FinishedAllTasks = TasksComplete == AllComponents.Length; // Has to be before inner while loop.
				CollisionInfo Collision;
				while(Collisions.TryPop(out Collision)) {
					if(!PreviousCollisions.Contains(Collision)) {
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
		private float _HorizontalDrag = 6000;
		private TimeSpan _MaxStep = TimeSpan.FromMilliseconds(10);
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
