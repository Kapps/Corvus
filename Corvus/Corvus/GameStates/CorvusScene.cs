using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Scenes;

namespace Corvus.GameStates {
	/// <summary>
	/// Represents a single level, or Scene, within Corvus.
	/// </summary>
	public class CorvusScene : Scene {

		/// <summary>
		/// Gets the name of this Scene.
		/// </summary>
		public string Name {
			get { return _Name; }
		}

		private CorvusScene(LevelData Data, string Name, GameState State) : base(Data, State) {
			this._Name = Name;
		}

		/// <summary>
		/// Loads and returns the scene with the given name.
		/// </summary>
		public static CorvusScene Load(string Name) {
			LevelData Data = LevelData.LoadTmx("Data/Levels/" + "Tutorial" + ".tmx");
			var SceneManager = CorvusGame.Instance.SceneManager;
			CorvusScene Result = new CorvusScene(Data, Name, SceneManager);
			return Result;
		}

		private string _Name;
	}
}
