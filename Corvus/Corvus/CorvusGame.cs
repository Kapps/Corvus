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
			if(SceneManager.ActiveScene != null)
				SceneManager.ActiveScene.AddEntity(obj.Character);
			var CameraComponent = new ChaseCameraComponent(obj.Camera);
			obj.Character.Components.Add(CameraComponent);
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
            AudioManager.SetMusicVolume(0f); //temp: just here cuz it's really annoying to debug and listen to the music.
            _SceneManager = new SceneManager();
            _SceneManager.ChangeScene("BasicLevel");
            // Start off in game.
            StateManager.PushState(_SceneManager);
            _MainMenuState = new MainMenuState();
           // StateManager.PushState(_MainMenuState); //TODO: Move this probably
            //AudioManager.PlayMusic("Title1");
            //AudioManager.SetMusicVolume(0.5f);
            _PausedState = new PausedState();
            CreateNewPlayer();
            MenuBinds(); //TODO: Not sure how to do Menu Binds, temp for now.
            RegisterGlobalComponent(new DebugComponent());
			GraphicsManager.ApplyChanges();
		}

		private void CreateNewPlayer() {
			// TODO: Allow new players to join by pressing a button. Should be simple enough.
			// TODO: Allow support for different 'classes' by just using different blueprints.
			var Blueprint = EntityBlueprint.GetBlueprint("Player");
			var PlayerEntity = Blueprint.CreateEntity();
            PlayerEntity.Size = new Vector2(48, 32);

            //Set spawn point, if any.
            //This is kinda weird, since we should have this all in once place.
            //We've basically got a spawn point handler in CorvusGame and SceneManager.
                //One is for creating a new player, and the other for changing the player's scene.
            var spawnPoint = SceneManager.ActiveScene.Entities.FirstOrDefault(c => c.Name == "Spawn Point");
            if (spawnPoint != null)
            {
                PlayerEntity.Position = spawnPoint.Position;
            }
            else
            {
                PlayerEntity.Position = new Vector2(1790, 1376);
            }

			CorvusPlayer Player = new CorvusPlayer(PlayerEntity);
			AddPlayer(Player);
			CorvusBinds.CreateBinds(Player); 
		}

        //possibly temp
        private void MenuBinds()
        {
            var player = this.Players.First();

            Bind Next = new Bind(player.InputManager, _MainMenuState.ControlManager.NextControl, false, Keys.Down);
            player.InputManager.RegisterBind(Next);
            Bind Prev = new Bind(player.InputManager, _MainMenuState.ControlManager.PreviousControl, false, Keys.Up);
            player.InputManager.RegisterBind(Prev);
            Bind Select = new Bind(player.InputManager, _MainMenuState.ControlManager.SelectControl, false, Keys.Enter);
            player.InputManager.RegisterBind(Select);
            //xbox controller
            Bind GamePadNext = new Bind(player.InputManager, _MainMenuState.ControlManager.NextControl, false, Buttons.LeftThumbstickDown);
            player.InputManager.RegisterBind(GamePadNext);
            Bind GamePadPrev = new Bind(player.InputManager, _MainMenuState.ControlManager.PreviousControl, false, Buttons.LeftThumbstickUp);
            player.InputManager.RegisterBind(GamePadPrev);
            Bind GamePadSelect = new Bind(player.InputManager, _MainMenuState.ControlManager.SelectControl, false, Buttons.A);
            player.InputManager.RegisterBind(GamePadSelect);
        }

        private SceneManager _SceneManager;
        private MainMenuState _MainMenuState;
        private PausedState _PausedState;

	}
}
