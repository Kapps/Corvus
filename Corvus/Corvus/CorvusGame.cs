using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Entities;
using CorvEngine.Entities.Blueprints;
using Corvus.Components;
using Corvus.GameStates;
using Microsoft.Xna.Framework;

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
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here
			this.RegisterGlobalComponent(new AudioManager(this.Game, @"Content\Audio\RpgAudio.xgs", @"Content\Audio\Wave Bank.xwb", @"Content\Audio\Sound Bank.xsb"));
			RegisterGlobalComponent(new DebugComponent());
			_SceneManager = new SceneManager();
			_SceneManager.ChangeScene("BasicLevel");
			// Start off in game.
			StateManager.PushState(_SceneManager);
			CreateNewPlayer();
		}

		private void CreateNewPlayer() {
			// TODO: Allow new players to join by pressing a button. Should be simple enough.
			// TODO: Allow support for different 'classes' by just using different blueprints.
			var Blueprint = EntityBlueprint.GetBlueprint("TestEntity");
			var PlayerEntity = Blueprint.CreateEntity();
			PlayerEntity.Size = new Vector2(64, 48);
			PlayerEntity.Position = new Vector2(PlayerEntity.Location.Width, 100);
			CorvusPlayer Player = new CorvusPlayer(PlayerEntity);
			AddPlayer(Player);
			CorvusBinds.CreateBinds(Player);
		}

		private SceneManager _SceneManager;
	}
}
