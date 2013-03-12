using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NodeType = System.Collections.Generic.LinkedListNode<CorvEngine.Components.Entity>;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides access to a scene that contains Entities for the game.
	/// </summary>
	public class Scene : GameStateComponent, IDisposable {
		// TODO: Should this really be a GameStateComponent?

		/// <summary>
		/// Gets an event called when this Scene is disposed.
		/// </summary>
		public event Action<Scene> Disposed;

		// TODO: This should be ObjectAdded and ObjectRemoved.
		/// <summary>
		/// An event raised when an Entity is added to this Scene, called after the Entity is initialized.
		/// </summary>
		public event Action<Entity> EntityAdded;
		
		/// <summary>
		/// An event raised when an Entity is removed from this Scene, called before the Entity is disposed.
		/// </summary>
		public event Action<Entity> EntityRemoved;

		/// <summary>
		/// Creates a Scene with the given LevelData.
		/// </summary>
		public Scene(LevelData Data, GameState State)
			: base(State) {

            this._Properties = Data.Properties;
			this._Layers = Data.Layers;
			foreach(var Entity in Data.DynamicObjects)
				AddEntity(Entity);

			this._MapSize = Data.MapSize;
			this._TileSize = Data.TileSize;
		}

        /// <summary>
        /// Gets the properties within this Scene.
        /// </summary>
		public IEnumerable<LevelProperty> Properties {
			get { return _Properties; }
		}

		/// <summary>
		/// Gets the layers present in this Scene.
		/// </summary>
		public IEnumerable<Layer> Layers {
			get { return _Layers; }
		}

		/// <summary>
		/// Gets the Entities contained within this Scene.
		/// </summary>
		public IEnumerable<Entity> Entities {
			get { return _Entities; }
		}

		/// <summary>
		/// Gets the size of this map, in world space coordinates.
		/// </summary>
		public Vector2 MapSize {
			get { return _MapSize; }
		}

		/// <summary>
		/// Gets the size of each tile within this map, in world space coordinates.
		/// </summary>
		public Vector2 TileSize {
			get { return _TileSize; }
		}

		/// <summary>
		/// Returns the number of tiles present in the map.
		/// </summary>
		public Vector2 TilesInMap {
			get { return MapSize / TileSize; }
		}

		/// <summary>
		/// Indicates if this Scene has been disposed of.
		/// Unlike Entities and Components, a Scene may only be disposed once.
		/// </summary>
		public bool IsDisposed {
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Indicates if this Scene has been initialized.
		/// Unlike Entities and Components, a Scene may only be initialized once.
		/// </summary>
		public bool IsInitialized {
			get { return _IsInitialized; }
		}

		// TODO: Merge these into AddObject and RemoveObject.

		/// <summary>
		/// Returns the smallest entity which contains the given point, or null if none were found to be located there.
		/// </summary>
		public Entity GetEntityAtPosition(Point Position) {
			return Entities.Where(c => c.Location.Contains(Position)).OrderBy(c => c.Location.Width * c.Location.Height).FirstOrDefault();
		}

		/// <summary>
		/// Adds the given Entity to this Scene.
		/// </summary>
		public void AddEntity(Entity Entity) {
			if(Entity.Scene != null)
				throw new ArgumentException("This Entity is already part of a different scene.");
			var Node = this._Entities.AddLast(Entity);
			Entity.NodeReference = new EntityNode(Entity, Node);
			if(IsInitialized)
				Entity.Initialize(this);
			if(this.EntityAdded != null)
				this.EntityAdded(Entity);
			Entity.Disposed += Entity_Disposed;
		}

		/// <summary>
		/// Adds the specified System to be part of this Scene.
		/// </summary>
		public void AddSystem(Components.SceneSystem System) {
			if(IsInitialized)
				System.Initialize(this);
			System.Disposed += System_Disposed;
			_Systems.Add(System);
		}

		/// <summary>
		/// Removes the specified System from this Scene.
		/// </summary>
		public void RemoveSystem(Components.SceneSystem System) {
			// Same as above, this should be considered temporary.
			_Systems.Remove(System);
			System.Dispose();
		}

		/// <summary>
		/// Returns the first System that matches or is derived from the specified type, or null if none match.
		/// </summary>
		public T GetSystem<T>() where T : SceneSystem {
			var Type = typeof(T);
			foreach(var System in _Systems)
				if(System.GetType() == Type || System.GetType().IsSubclassOf(Type))
					return (T)System;
			return null;
		}

		/// <summary>
		/// Initializes the Scene and all Systems and Entities within it.
		/// This may only ever be called once on a Scene.
		/// </summary>
		public void Initialize() {
			if(_IsInitialized)
				throw new InvalidOperationException("Unable to initialize a Scene multiple times.");
			_IsInitialized = true;
			foreach(var System in _Systems)
				System.Initialize(this);
			foreach(var Entity in _Entities)
				Entity.Initialize(this);
		}

		/// <summary>
		/// Disposes of this Scene, removing all remaining Entities and Systems.
		/// Unlike SceneObjects, a Scene may not be initialized after being disposed of.
		/// </summary>
		public void Dispose() {
			if(_IsDisposed)
				throw new InvalidOperationException("Unable to dispose a Scene multiple times.");
			_IsDisposed = true;
			var DupedEntities = _Entities.ToArray();
			foreach(var Entity in DupedEntities)
				Entity.Dispose();
			if(this.Disposed != null)
				this.Disposed(this);
		}

		protected override void OnDraw(GameTime Time) {
			// TODO: Call this once for each player after setting Viewport and Camera.
			// TODO: Would be nice to do some sort of spacial partitioning here, but we don't yet.
			var GraphicsDevice = CorvBase.Instance.GraphicsDevice;
			foreach(var Player in CorvBase.Instance.Players) {
				Camera.Active = Player.Camera;
				Viewport Viewport = new Viewport(0, 0, (int)Camera.Active.Size.X, (int)Camera.Active.Size.Y);
				var StartTile = Camera.Active.Position / TileSize;
				var EndTile = (Camera.Active.Position + Camera.Active.Size) / TileSize;
				var SpriteBatch = CorvBase.Instance.SpriteBatch;

				int StartX = Math.Max((int)StartTile.X, 0);
				int StartY = Math.Max((int)StartTile.Y, 0);
				int EndX = Math.Min((int)(EndTile.X + 0.5f), (int)TilesInMap.X - 1);
				int EndY = Math.Min((int)(EndTile.Y + 0.5f), (int)TilesInMap.Y - 1);
				foreach(var Layer in _Layers) {
					for(int y = StartY; y <= EndY; y++) {
						for(int x = StartX; x <= EndX; x++) {
							Tile Tile = Layer.GetTile(x, y);
							if(Tile == null)
								continue;
							var ScreenCoords = Camera.Active.ScreenToWorld(new Vector2(Tile.Location.X, Tile.Location.Y));
							SpriteBatch.Draw(Tile.Texture, new Rectangle((int)ScreenCoords.X, (int)ScreenCoords.Y, Tile.Location.Width, Tile.Location.Height), Tile.SourceRect, Color.White);
						}
					}
				}
				foreach(var Entity in _Entities) {
					if(Camera.Active.Contains(Entity) && !Entity.IsDisposed)
						Entity.Draw();
				}
				foreach(var System in _Systems)
					if(!System.IsDisposed)
						System.Draw();
			}
		}

		protected override void OnUpdate(GameTime Time) {
			// Create a copy of our systems and entities so we can update without worry of disposed ones.
			// We can of course easily optimize this if need be.
			var EntitiesCopy = new Entity[_Entities.Count];
			var SystemsCopy = new SceneSystem[_Systems.Count];
			_Entities.CopyTo(EntitiesCopy, 0);
			_Systems.CopyTo(SystemsCopy, 0);
			foreach(var Entity in EntitiesCopy)
				if(!Entity.IsDisposed)
					Entity.Update(Time);
			foreach(var System in SystemsCopy)
				if(!System.IsDisposed)
					System.Update(Time);
		}

		private void Entity_Disposed(SceneObject obj) {
			Entity Entity = (Entity)obj;
			Entity.Disposed -= Entity_Disposed;
			if(this.EntityRemoved != null)
				this.EntityRemoved(Entity);
			var Node = (NodeType)Entity.NodeReference.Node;
			_Entities.Remove(Node);
		}

		private void System_Disposed(SceneObject obj) {
			_Systems.Remove((SceneSystem)obj);
		}

        private LevelProperty[] _Properties;
		private Layer[] _Layers;
		private Vector2 _MapSize;
		private Vector2 _TileSize;
		private List<Components.SceneSystem> _Systems = new List<Components.SceneSystem>();
		private bool _IsDisposed;
		private bool _IsInitialized;
		// TODO: Eventually, this should be changed to use a QuadTree or Grid, but for now we don't need the performance.
		// Support does exist for plugging one in efficiently using an EntityNode reference though.
		private LinkedList<Entity> _Entities = new LinkedList<Entity>();
	}	
}
