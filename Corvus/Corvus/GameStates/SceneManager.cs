using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Components.Blueprints;
using CorvEngine.Geometry;
using CorvEngine.Scenes;
using Corvus.Components;
using Corvus.Interface;

namespace Corvus.GameStates {

	/// <summary>
	/// A delegate called when a scene is changed.
	/// Note that OldScene may be null if the state just switched to the SceneManager loading a Scene for the first time.
	/// </summary>
	public delegate void SceneChangedDelegate(CorvusScene OldScene, CorvusScene NewScene);

	/// <summary>
	/// Provides a GameState that manages scenes within the game.
	/// </summary>
	public class SceneManager : GameState {

		/// <summary>
		/// Gets an event called when a Scene is changed.
		/// </summary>
		public event SceneChangedDelegate SceneChanged;

		public override bool BlocksUpdate {
			get { return true; }
		}

		public override bool BlocksDraw {
			get { return true; }
		}

		/// <summary>
		/// Gets the currently active Scene in the game.
		/// </summary>
		public CorvusScene ActiveScene {
			get { return _ActiveScene; }
		}

		/// <summary>
		/// Creates a new instance of the SceneManager, loading all entitiy blueprints in preparation for Scene changes.
		/// </summary>
		public SceneManager()
			: base() {
			ReloadBlueprints();
		}

		/// <summary>
		/// Changes the scene to the Scene with the specified name.
		/// If the Scene has already been loaded, it will reuse that instance of the scene.
		/// Otherwise, the Scene will be loaded.
		/// </summary>
		public CorvusScene ChangeScene(string LevelName, bool ResetPlayers) {
			// TODO: Refactor this.
			var Scene = ActiveScenes.FirstOrDefault(c => c.Name.Equals(LevelName, StringComparison.InvariantCultureIgnoreCase));
			if(Scene == null) {
				Scene = CorvusScene.Load(LevelName);
				Scene.Disposed += Scene_Disposed;
				ActiveScenes.Add(Scene);
				Scene.AddSystem(new PhysicsSystem());

				//This is likely a shitty idea for obvious reasons, but still works.
				if(Scene.Name.Contains("Arena"))
					Scene.AddSystem(new ArenaSystem());


				TiledPlatformerGeometry Geometry = new TiledPlatformerGeometry(Scene);
				Scene.Initialize(Geometry);
				this.AddComponent(Scene);
				this.AddComponent(new Corvus.Interface.InGameInterface(Scene.GameState)); //TODO: Possibly the wrong spot?
			}
			var OldScene = _ActiveScene;
			if(OldScene == Scene)
				return Scene;
			_ActiveScene = Scene;
			if(SceneChanged != null)
				SceneChanged(OldScene, _ActiveScene);
			if(OldScene != null) {
				OldScene.Visible = false;
				OldScene.Enabled = false;
			}
			ActiveScene.Visible = true;
			ActiveScene.Enabled = true;
			foreach(CorvusPlayer Player in CorvusGame.Instance.Players) {
				if(Player.Character != null) {
					if(ResetPlayers)
						Player.ResetCharacter();
					else if(!Player.Character.IsDisposed) // Dispose it to transfer to a new Scene.
						Player.Character.Dispose();
					var Position = Player.Character.Position;
					ActiveScene.AddEntity(Player.Character);
					Player.Character.Position = Position; // Keep them at their old position.
				}
			}

			//Plays the song. Not sure if it should be here.
			//Plays the song. Not sure if it should be here.
			if(Scene.Properties.Any()) {
				foreach(LevelProperty p in Scene.Properties) {
					if(p.Name.Equals("Audio", StringComparison.InvariantCultureIgnoreCase)) {
						var songProperties = p.Value.Split(',');
						if(songProperties.Length >= 1 && string.IsNullOrEmpty(songProperties[0])) {
							CorvEngine.AudioManager.StopMusic(); //Stops the song 
							continue;
						} else if(songProperties.Length != 3)
							throw new ArgumentException("Expected three arguments for Audio, being the song name, fade duration, and volume. Ex:(SongName1, 2, 0.5)");
						string songName = songProperties[0];
						float fadeDuration = float.Parse(songProperties[1]);
						float volume = float.Parse(songProperties[2]);
						CorvEngine.AudioManager.PlayMusic(songName, fadeDuration);
						CorvEngine.AudioManager.SetMusicVolume(volume);
					}
				}
			}

			//Set spawn point.
			//This is kinda weird, since we should have this all in once place.
			//We've basically got a spawn point handler in CorvusGame and SceneManager.
			//One is for creating a new player, and the other for changing the player's scene.
			var spawnPoint = Scene.Entities.FirstOrDefault(c => c.Name == "Spawn Point");
			if(Scene.Engine.Players.Count() != 0) {
				foreach(var player in Scene.Engine.Players) {
					var playerEntity = player.Character;

					if(spawnPoint != null) {
						playerEntity.Position = spawnPoint.Position;
					}
				}
			}

			return _ActiveScene;
		}

		void Scene_Disposed(Scene obj) {
			ActiveScenes.Remove((CorvusScene)obj);
			this.RemoveComponent(obj);
		}

		/// <summary>
		/// Reloads all EntityBlueprints from files in the Data folder.
		/// </summary>
		public void ReloadBlueprints() {
			foreach(var BlueprintFile in Directory.GetFiles("Data/Entities", "*.txt"))
				BlueprintParser.ParseBlueprint(File.ReadAllText(BlueprintFile));
		}

		/// <summary>
		/// Reloads all scenes, including the currently active scene.
		/// If no scene is active (and thus no scenes are loaded), this method returns immediately.
		/// </summary>
		public void ReloadScenes(bool ResetPlayers) {
			if(ActiveScenes.Count == 0)
				return;
			// First remove all players from the active scene so we don't dispose them.
			var PlayerChars = CorvusGame.Instance.Players.Select(c => c.Character);
			string ActiveName = ActiveScene.Name;
			foreach(var PlayerChar in PlayerChars)
				if(!PlayerChar.IsDisposed)
					PlayerChar.Dispose();
			string CurrentSceneName = _ActiveScene.Name;
			foreach(var Scene in ActiveScenes.ToArray()) // Duplicate so can modify.
				Scene.Dispose();
			_ActiveScene = null;
			ChangeScene(ActiveName, ResetPlayers);
		}

		private CorvusScene _ActiveScene;
		private List<CorvusScene> ActiveScenes = new List<CorvusScene>();
	}
}