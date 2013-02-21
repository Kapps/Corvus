using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Input;
using Corvus.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Corvus.Components {
	/// <summary>
	/// A global component used to display debugging information, such as frame rate and position.
	/// </summary>
	public class DebugComponent : DrawableGameComponent {
		private SpriteFont Font;
		const string DATA_FOLDER_PATH = "../../../Data";
		public DebugComponent() : base(CorvusGame.Instance.Game) {
			this.Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/TestFont");
			CorvusGame.Instance.PlayerAdded += Instance_PlayerAdded;
			if(Directory.Exists(DATA_FOLDER_PATH)) {
				FileSystemWatcher fsw = new FileSystemWatcher(DATA_FOLDER_PATH);
				fsw.InternalBufferSize = 1024 * 256;
				fsw.IncludeSubdirectories = true;
				fsw.Changed += DataFileUpdate;
				fsw.Created += DataFileUpdate;
				fsw.EnableRaisingEvents = true;
			}
		}

		void DataFileUpdate(object sender, FileSystemEventArgs e) {
			var FileUri = new Uri(Path.GetFullPath(e.FullPath));
			var DataUri = new Uri(Path.GetFullPath(DATA_FOLDER_PATH));
			var RelativeUri = DataUri.MakeRelativeUri(FileUri);
			string SourcePath = FileUri.AbsolutePath;
			string DestinationPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/" + RelativeUri.OriginalString);
			File.Copy(SourcePath, DestinationPath, true);
		}

		void Instance_PlayerAdded(Player Player) {
			Bind Bind = new Bind(Player.InputManager, ReloadLevel, false);
			Bind.RegisterButton(new InputButton(Keys.F5));
			Player.InputManager.RegisterBind(Bind);
		}

		public override void Draw(GameTime gameTime) {
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "FPS: " + CorvusGame.Instance.FPS, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow);
			if(CorvusGame.Instance.Players.Any())
				CorvusGame.Instance.SpriteBatch.DrawString(Font, "Position: " + CorvusGame.Instance.Players.First().Character.Position, new Vector2(10, GraphicsDevice.Viewport.Height - 50), Color.Yellow);
			base.Draw(gameTime);
		}

		private void ReloadLevel(BindState State) {
			if(State == BindState.Released)
				return;
			CorvusGame.Instance.SceneManager.ReloadScenes();
		}
	}
}
