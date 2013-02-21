using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Entities.Blueprints;
using CorvEngine.Scenes;

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
		public SceneManager() : base() {
			foreach(var BlueprintFile in Directory.GetFiles("Data/Entities", "*.txt"))
				BlueprintParser.ParseBlueprint(File.ReadAllText(BlueprintFile));
		}

		/// <summary>
		/// Changes the scene to the Scene with the specified name.
		/// If the Scene has already been loaded, it will reuse that instance of the scene.
		/// Otherwise, the Scene will be loaded.
		/// </summary>
		public CorvusScene ChangeScene(string LevelName) {
			var Scene = ActiveScenes.FirstOrDefault(c => c.Name.Equals(LevelName, StringComparison.InvariantCultureIgnoreCase));
			if(Scene == null) {
				Scene = CorvusScene.Load(LevelName);
				ActiveScenes.Add(Scene);
				Scene.AddSystem(new PhysicsSystem());
				this.AddComponent(Scene);
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
			foreach(var Player in CorvusGame.Instance.Players) {
				if(Player.Character != null) {
					if(OldScene != null)
						OldScene.RemoveEntity(Player.Character);
					ActiveScene.AddEntity(Player.Character);
					Player.Character.Position = new Microsoft.Xna.Framework.Vector2(Player.Character.Location.Width + 10, Player.Character.Location.Height + 100);
				}
			}
			return _ActiveScene;
		}

		/// <summary>
		/// Reloads all scenes, including the currently active scene.
		/// If no scene is active (and thus no scenes are loaded), this method returns immediately.
		/// </summary>
		public void ReloadScenes() {
			if(ActiveScenes.Count == 0)
				return;
			// First remove all players from the active scene so we don't dispose them.
			var PlayerChars = CorvusGame.Instance.Players.Select(c => c.Character);
			string ActiveName = ActiveScene.Name;
			foreach(var PlayerChar in PlayerChars)
				_ActiveScene.RemoveEntity(PlayerChar);
			string CurrentSceneName = _ActiveScene.Name;
			foreach(var Scene in ActiveScenes)
				Scene.Dispose();
			_ActiveScene = null;
			ChangeScene(ActiveName);
		}

		private CorvusScene _ActiveScene;
		private List<CorvusScene> ActiveScenes = new List<CorvusScene>();
	}
}
