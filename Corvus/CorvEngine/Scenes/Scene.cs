﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using NodeType = System.Collections.Generic.LinkedListNode<CorvEngine.Entities.Entity>;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides access to a scene that contains Entities for the game.
	/// </summary>
	public class Scene : GameStateComponent, IDisposable {
		// TODO: Should this really be a GameStateComponent?

		// TODO: Eventually, this should be changed to use a QuadTree or Grid, but for now we don't need the performance.
		// Support does exist for plugging one in efficiently using an EntityNode reference though.
		private LinkedList<Entity> _Entities = new LinkedList<Entity>();

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

			this._Layers = Data.Layers;
			foreach(var Entity in Data.DynamicObjects)
				AddEntity(Entity);

			this._MapSize = Data.MapSize;
			this._TileSize = Data.TileSize;
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
		/// Adds the given Entity to this Scene.
		/// </summary>
		public void AddEntity(Entity Entity) {
			if(Entity.Scene != null)
				throw new ArgumentException("This Entity is already part of a different scene.");
			var Node = this._Entities.AddLast(Entity);
			Entity.NodeReference = new EntityNode(Entity, Node);
			Entity.Initialize(this);
			if(this.EntityAdded != null)
				this.EntityAdded(Entity);
		}

		/// <summary>
		/// Removes the given Entity from this Scene.
		/// </summary>
		/// <param name="Entity"></param>
		public void RemoveEntity(Entity Entity) {
			if(this.EntityRemoved != null)
				this.EntityRemoved(Entity);
			var Node = (NodeType)Entity.NodeReference.Node;
			_Entities.Remove(Node);
			Entity.NodeReference = null;
		}

		/// <summary>
		/// Adds the specified System to be part of this Scene.
		/// </summary>
		public void AddSystem(Components.System System) {
			// TODO: Should really share a common type/interface with Entity...
			_Systems.Add(System);
		}

		/// <summary>
		/// Removes the specified System from this Scene.
		/// </summary>
		public void RemoveSystem(Components.System System) {
			// Same as above, this should be considered temporary.
			_Systems.Remove(System);
		}

		/// <summary>
		/// Disposes of this Scene, removing all remaining Entities.
		/// </summary>
		public void Dispose() {
			foreach(var Entity in _Entities)
				RemoveEntity(Entity);
		}

		protected override void OnDraw(GameTime Time) {
			// TODO: Call this once for each player after setting Viewport and Camera.
			// TODO: Would be nice to do some sort of spacial partitioning here, but we don't yet.
			foreach(var Player in CorvBase.Instance.Players) {
				Camera.Active = Player.Camera; // TODO: These should all be different Viewports.
				var Viewport = CorvBase.Instance.GraphicsDevice.Viewport;
				Camera.Active.Size = new Vector2(Viewport.Width, Viewport.Height);
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
					if(Camera.Active.Contains(Entity))
						Entity.Draw();
				}
				foreach(var System in _Systems)
					System.NotifyDraw();
			}
		}

		protected override void OnUpdate(GameTime Time) {
			foreach(var Entity in _Entities)
				Entity.Update(Time);
			foreach(var System in _Systems)
				System.NotifyUpdate(Time);
		}

		private Layer[] _Layers;
		private Vector2 _MapSize;
		private Vector2 _TileSize;
		private List<Components.System> _Systems = new List<Components.System>();
	}	
}
