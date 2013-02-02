using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
	/// The CorvEngine class does not derive from Game, as when using CorvEngine the Game class is not used directly.
	/// Instead, GameEngine wraps Game class, forwarding methods to it as needed.
	/// </summary>
	public class CorvEngine : IDisposable {
		private List<Player> _Players;
		private static CorvEngine _Instance;
		private Game _Game;
		private GraphicsDeviceManager _GraphicsManager;
		private GraphicsDevice _GraphicsDevice;
		private SpriteBatch _SpriteBatch;
		private bool _Paused;

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
		public static CorvEngine Instance {
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
			get { return _GraphicsDevice; }
		}

		/// <summary>
		/// Gets the instance of the Game that CorvEngine is wrapping.
		/// This is used primarily for creating GameComponents for global components.
		/// </summary>
		public Game Game {
			get { return _Game; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the game is currently paused.
		/// All global components will continue to run when the game is paused, as well as any StateComponents with RunWhenPaused set to true.
		/// </summary>
		public bool Paused {
			get { return _Paused; }
			set { _Paused = value; }
		}

		/// <summary>
		/// Creates a new instance of the GameEngine.
		/// Only one instance of the GameEngine may exist at any time.
		/// </summary>
		public CorvEngine() {
			if(Interlocked.Exchange(ref _Instance, this) != null)
				throw new InvalidOperationException("Only one instance of GameEngine may exist at any time.");
			this._Players = new List<Player>();
			this._Game = new Game();
			this._GraphicsManager = new GraphicsDeviceManager(this._Game);
			this._GraphicsDevice = this._Game.GraphicsDevice;
			this._SpriteBatch = new SpriteBatch(GraphicsDevice);
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
		/// Registers the given GameComponent as a global GameComponent.
		/// </summary>
		public void RegisterGlobalComponent(IGameComponent Component) {
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
	}
}
