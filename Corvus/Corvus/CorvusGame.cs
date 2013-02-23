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
			_SceneManager = new SceneManager();
			_SceneManager.ChangeScene("BasicLevel");
			// Start off in game.
			StateManager.PushState(_SceneManager);
			CreateNewPlayer();
			RegisterGlobalComponent(new DebugComponent());
			GraphicsManager.ApplyChanges();
		}

		private void CreateNewPlayer() {
			// TODO: Allow new players to join by pressing a button. Should be simple enough.
			// TODO: Allow support for different 'classes' by just using different blueprints.
			var Blueprint = EntityBlueprint.GetBlueprint("TestEntity");
			var PlayerEntity = Blueprint.CreateEntity();
			PlayerEntity.Size = new Vector2(48, 32);
			PlayerEntity.Position = new Vector2(1790, 1376);
			CorvusPlayer Player = new CorvusPlayer(PlayerEntity);
			AddPlayer(Player);
			CorvusBinds.CreateBinds(Player);
		}

		private SceneManager _SceneManager;
	}
}
