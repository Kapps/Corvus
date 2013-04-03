using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using CorvEngine.Graphics;
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
		private SpriteBatch _SpriteBatch;
		private FrameInvoker _FrameInvoker;
		private float _TimeSinceFPSUpdate = 0;
		private int _CurrentFrames;
		private int _FPS;
		private TimeSpan _MaxStepDuration = TimeSpan.FromMilliseconds(25f);
		private TimeSpan _FrameSkipDuration = TimeSpan.FromMilliseconds(1000);
		//private bool _Paused;

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
		/// Gets or sets a value indicating whether the game is currently paused.
		/// All global components will continue to run when the game is paused, as well as any StateComponents with RunWhenPaused set to true.
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
			get { return Game.Services; }
		}

		/// <summary>
		/// Gets a global SpriteBatch instance that may be used for rendering.
		/// This SpriteBatch is expected to always remain in the Begin state during a draw phase.
		/// </summary>
		public SpriteBatch SpriteBatch {
			get { return _SpriteBatch; }
		}

		/// <summary>
		/// Returns a ContentManager used for loading assets that should not be GameState dependent.
		/// </summary>
		public ContentManager GlobalContent {
			get { return Game.Content; }
		}

		/// <summary>
		/// Gets a global component that can be used to invoke a command exactly once per frame.
		/// This is particularly useful when using events that start and stop triggering a command, such as movement binds.
		/// </summary>
		public FrameInvoker FrameInvoker {
			get { return _FrameInvoker; }
		}

		/// <summary>
		/// Gets or sets the maximum amount of time that should pass in a single update.
		/// If a greater amount than this value, but less than FrameSkipDuration occurs since the last frame, it is split into multiple steps.
		/// This property only applies for Update. Draw is still called only once.
		/// </summary>
		public TimeSpan MaxStepDuration {
			get { return _MaxStepDuration; }
			set { _MaxStepDuration = value; }
		}

		/// <summary>
		/// Gets or sets the maximum amount of time that a frame should simulate without being discarded.
		/// If greater than this value passes since the last frame, this frame call is discarded (both Update and Draw).
		/// </summary>
		public TimeSpan FrameSkipDuration {
			get { return _FrameSkipDuration; }
			set { _FrameSkipDuration = value; }
		}

		/// <summary>
		/// Gets the number of Update calls that occurred in the last second.
		/// </summary>
		public int FPS {
			get { return _FPS; }
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
			this.RegisterGlobalComponent(Player.InputManager);
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
			this.RemoveGlobalComponent(Player.InputManager);
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
			this._GraphicsManager.SynchronizeWithVerticalRetrace = false;
			/*
#if !DEBUG
			
			//this._GraphicsManager.PreferMultiSampling = true;
#if WINDOWS
			IntPtr hWnd = Game.Window.Handle;
			var control = System.Windows.Forms.Control.FromHandle(hWnd);
			var form = control.FindForm();
			form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
#endif
			this._GraphicsManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
			this._GraphicsManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
			this._GraphicsManager.ApplyChanges();
			//this._GraphicsManager.ToggleFullScreen();
#else
			this._GraphicsManager.PreferredBackBufferWidth = 1024;
			this._GraphicsManager.PreferredBackBufferHeight = 768;
			this._GraphicsManager.ApplyChanges();
#endif
			 */
			this._GraphicsManager.PreferredBackBufferWidth = 1024;
			this._GraphicsManager.PreferredBackBufferHeight = 768;
			this._GraphicsManager.ApplyChanges();
			this._SpriteBatch = new SpriteBatch(GraphicsDevice);
			this._StateManager = new GameStateManager(_Game);
			this._FrameInvoker = new FrameInvoker();

			this.RegisterGlobalComponent(this._FrameInvoker);
			this.RegisterGlobalComponent(this._StateManager);
			this.Initialize();
		}

		private class CorvusInternalGame : Game {
			private CorvBase _Engine;

			public CorvusInternalGame(CorvBase Engine) {
				this._Engine = Engine;
				Content.RootDirectory = "Content";
				var GraphicsManager = new GraphicsDeviceManager(this);
				CorvBase.Instance._GraphicsManager = GraphicsManager;
				this.IsFixedTimeStep = false;
				this.TargetElapsedTime = TimeSpan.FromMilliseconds(1f);
			}

			protected override void Initialize() {
				base.Initialize();
				_Engine.OnInitialize();
			}

			protected override void Update(GameTime gameTime) {
				//try {
				if(gameTime.ElapsedGameTime >= _Engine.FrameSkipDuration) {
					Thread.Sleep(5); // Sleep 5ms so we don't have a near-instant tick.
					return;
				}
				TimeSpan Remaining = gameTime.ElapsedGameTime;
				while(Remaining > TimeSpan.Zero) {
					TimeSpan Delta = TimeSpan.FromTicks(Math.Min(_Engine.MaxStepDuration.Ticks, Remaining.Ticks));
					GameTime StepTime = new GameTime(gameTime.TotalGameTime.Subtract(Remaining), Delta);
					Remaining = Remaining.Subtract(Delta);
					base.Update(StepTime);
				}
				
				/*} catch(Exception ex) {
					// TODO: Do something here.
					// Like, display a UI mbox asking if they wish to continue.
					// Also, allow actually handling what happens on an exception.
					// Ideally, it'd log the exception, maybe upload to a server, and then either continue or quit.
					// If the same exception type is received each frame, it won't log it multiple times.
					if(Debugger.IsAttached)
						Console.WriteLine(ex);
				}*/
			}

			protected override void Draw(GameTime gameTime) {
				if(gameTime.ElapsedGameTime >= _Engine.FrameSkipDuration)
					return;
				var Instance = CorvBase._Instance;
				Instance._TimeSinceFPSUpdate += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				Instance._CurrentFrames++;
				if(Instance._TimeSinceFPSUpdate > 1000) {
					Instance._FPS = Instance._CurrentFrames;
					Instance._TimeSinceFPSUpdate -= 1000;
					Instance._CurrentFrames = 0;
				}
				GraphicsDevice.Clear(Color.DarkSlateGray);
				CorvBase.Instance.SpriteBatch.Begin();
				base.Draw(gameTime);
				CorvBase.Instance.SpriteBatch.End();
			}
		}
	}
}
