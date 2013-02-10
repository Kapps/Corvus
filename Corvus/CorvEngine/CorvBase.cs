using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CorvEngine {
	/// <summary>
	/// Provides the main engine to run for the Game.
	/// The Engine class does not derive from Game, as when using CorvEngine the Game class is not used directly.
	/// Instead, GameEngine wraps Game class, forwarding methods to it as needed.
	/// </summary>
	public abstract class CorvBase : IDisposable {
		private List<Player> _Players;
		private static CorvBase _Instance;
		private CorvusInternalGame _Game;
		private GraphicsDeviceManager _GraphicsManager;
		private GameStateManager _StateManager;

		/// <summary>
		/// Gets an event called when a new Player is added to the game.
		/// </summary>
		public event Action<Player> PlayerAdded;

		/// <summary>
		/// Gets an event called when a Player is removed from the game.
		/// </summary>
		public event Action<Player> PlayerRemoved;

		/// <summary>
		/// Returns the singleton instance of CorvEngine.
		/// </summary>
		public static CorvBase Instance {
			get { return _Instance; }
		}
		
		/// <summary>
		/// Gets the players that are currently participating in the game.
		/// Until players are added, there are zero players in the game.
		/// </summary>
		public IEnumerable<Player> Players {
			get { return _Players; }
		}

		/// <summary>
		/// Gets the GraphicsDeviceManager being used for the game.
		/// </summary>
		public GraphicsDeviceManager GraphicsManager {
			get { return _GraphicsManager; }
		}

		/// <summary>
		/// Gets the GraphicsDevice being used for the game.
		/// </summary>
		public GraphicsDevice GraphicsDevice {
			get { return Game.GraphicsDevice; }
		}

		/// <summary>
		/// Gets the instance of the Game that CorvEngine is wrapping.
		/// This is used primarily for creating GameComponents for global components.
		/// </summary>
		public Game Game {
			get { return _Game; }
		}

		/// <summary>
		/// Gets the GameStateManager used by the engine.
		/// </summary>
		public GameStateManager StateManager {
			get { return _StateManager; }
		}

		/// <summary>
		/// Gets a service provider used by the underlying Game.
		/// </summary>
		public IServiceProvider Services {
			get { return _Game.Services; }
		}

		/// <summary>
		/// Creates a new instance of the GameEngine.
		/// Only one instance of the GameEngine may exist at any time.
		/// </summary>
		public CorvBase() {
			if(Interlocked.Exchange(ref _Instance, this) != null)
				throw new InvalidOperationException("Only one instance of GameEngine may exist at any time.");
			this._Players = new List<Player>();
			this._Game = new CorvusInternalGame(this);
		}

		/// <summary>
		/// Starts the game.
		/// </summary>
		public void Run() {
			this._Game.Run();
		}

		/// <summary>
		/// Exits the game.
		/// </summary>
		public void Exit() {
			this._Game.Exit();
		}

		/// <summary>
		/// Initializes any logic required for the Game.
		/// </summary>
		protected abstract void Initialize();
		
		/// <summary>
		/// Registers the given GameComponent as a global GameComponent.
		/// </summary>
		public void RegisterGlobalComponent(IGameComponent Component) {
			Component.Initialize();
			_Game.Components.Add(Component);
		}

		/// <summary>
		/// Unregisters the given GameComponent, causing it to no longer be active.
		/// </summary>
		public void RemoveGlobalComponent(IGameComponent Component) {
			bool result = _Game.Components.Remove(Component);
			if(!result)
				throw new ArgumentException("The given component was not registered.");
		}

		/// <summary>
		/// Adds the specified new player to the game.
		/// </summary>
		public void AddPlayer(Player Player) {
			if(Players.Contains(Player))
				throw new ArgumentException("The given Player was already part of the game.");
			this._Players.Add(Player);
			if(this.PlayerAdded != null)
				this.PlayerAdded(Player);
		}

		/// <summary>
		/// Removes the given Player from the game.
		/// </summary>
		public void RemovePlayer(Player Player) {
			bool removed = _Players.Remove(Player);
			if(!removed)
				throw new KeyNotFoundException("The given Player was not registered with the game engine.");
			if(this.PlayerRemoved != null)
				this.PlayerRemoved(Player);
		}

		/// <summary>
		/// Disposes of the engine, exiting the game.
		/// </summary>
		void IDisposable.Dispose() {
			_Game.Dispose();
		}

		private void OnInitialize() {
			this._GraphicsManager = new GraphicsDeviceManager(this._Game);
			this._GraphicsManager.PreferredBackBufferWidth = 1024;
			this._GraphicsManager.PreferredBackBufferHeight = 768;
			this._GraphicsManager.ApplyChanges();
			this._StateManager = new GameStateManager(_Game);
			this.RegisterGlobalComponent(this._StateManager);
			this.Initialize();
		}

		private class CorvusInternalGame : Game {
			private CorvBase _Engine;

			public CorvusInternalGame(CorvBase Engine) {
				this._Engine = Engine;
			}

			protected override void Initialize() {
				base.Initialize();
				_Engine.OnInitialize();
			}
		}
	}
}
