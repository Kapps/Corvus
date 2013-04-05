using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Components;
using CorvEngine.Components.Blueprints;
using Corvus.Components;
using Corvus.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CorvEngine.Input;


namespace Corvus {
	/// <summary>
	/// Provides the main class for Corvus.
	/// </summary>
	public class CorvusGame : CorvBase {

		/// <summary>
		/// Returns the instance of the game, casted to CorvusGame.
		/// </summary>
		public static new CorvusGame Instance {
			get { return (CorvusGame)CorvBase.Instance; }
		}

		/// <summary>
		/// Gets the SceneManager used to display levels in the game.
		/// </summary>
		public SceneManager SceneManager {
			get { return _SceneManager; }
		}

        /// <summary>
        /// Gets the Main menu state.
        /// </summary>
        public MainMenuState MainMenuState { get { return _MainMenuState; } }

        /// <summary>
        /// Gets the paused state used in game.
        /// </summary>
        public PausedState PausedState { get { return _PausedState; } }

        /// <summary>
        /// Gets the options state used in game.
        /// </summary>
        public OptionsState OptionsState { get { return _OptionsState; } }

		/// <summary>
		/// Creates a new instance of CorvusGame without yet initializing it.
		/// </summary>
		public CorvusGame() {
			PlayerAdded += CorvusGame_PlayerAdded;
			PlayerRemoved += CorvusGame_PlayerRemoved;
		}

		void CorvusGame_PlayerRemoved(Player obj) {
			if(obj.Character != null)
				obj.Character.Dispose();
		}

		void CorvusGame_PlayerAdded(Player obj) {
			obj.CharacterChanged += (c) => SetCamera(obj);
			SetCamera(obj);
		}

		private void SetCamera(Player Player) {
			var CameraComponent = new ChaseCameraComponent(Player.Camera);
			Player.Character.Components.Add(CameraComponent);

			/*float AspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
			Vector2 Size;
			if(Math.Abs(AspectRatio - (16f / 9f)) < 0.01f)
				Size = new Vector2(1600, 900);
			else if(Math.Abs(AspectRatio - (16f / 10f)) < 0.01f)
				Size = new Vector2(1680, 1050);
			else
				Size = new Vector2(1600, 900);
			obj.Camera.Size = Size;*/
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here
			this.RegisterGlobalComponent(new AudioManager(this.Game, @"Content\Audio\RpgAudio.xgs", @"Content\Audio\Wave Bank.xwb", @"Content\Audio\Sound Bank.xsb"));
            _SceneManager = new SceneManager();
            _MainMenuState = new MainMenuState();
            _PausedState = new PausedState();
            _OptionsState = new OptionsState();
#if DEBUG
            AudioManager.Instance.MusicEnabled = false;
#endif
            // Start off in game.
            //_SceneManager.ChangeScene("BasicLevel", false);
            //StateManager.PushState(_SceneManager);
            StateManager.PushState(_MainMenuState); //TODO: Move this probably
            AudioManager.PlayMusic("Title1");
            AudioManager.SetMusicVolume(0.5f);
            CreateNewPlayer();
			GraphicsManager.ApplyChanges();

//#if DEBUG
			_SceneManager.AddComponent(new DebugComponent(_SceneManager));
//#endif
        }

		public void CreateNewPlayer() {
			// TODO: Allow new players to join by pressing a button. Should be simple enough.
			var PlayerEntity = CorvusPlayer.LoadPlayerEntity();
			CorvusPlayer Player = new CorvusPlayer(PlayerEntity);
			AddPlayer(Player);
			CorvusBinds.CreateBinds(Player); 
		}

        private SceneManager _SceneManager;
        private MainMenuState _MainMenuState;
        private PausedState _PausedState;
        private OptionsState _OptionsState;

	}
}
