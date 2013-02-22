using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
			if(Directory.Exists(DATA_FOLDER_PATH)) {
				FileSystemWatcher fsw = new FileSystemWatcher(DATA_FOLDER_PATH);
				fsw.InternalBufferSize = 1024 * 256;
				fsw.IncludeSubdirectories = true;
				fsw.Changed += DataFileUpdate;
				fsw.Created += DataFileUpdate;
				fsw.EnableRaisingEvents = true;
			}
			SceneManager = CorvusGame.Instance.SceneManager;
			CorvusGame.Instance.Game.IsMouseVisible = true;

			this.Player = (CorvusPlayer)CorvusGame.Instance.Players.First();
			Bind ReloadLevelsBind = new Bind(Player.InputManager, ReloadPressed, false, Keys.F5);
			Player.InputManager.RegisterBind(ReloadLevelsBind);
			Bind ClearCameraBind = new Bind(Player.InputManager, ClearCameraPressed, false, Keys.F12);
			Player.InputManager.RegisterBind(ClearCameraBind);
			CurrentCamera = Player.Character.GetComponent<ChaseCameraComponent>();
		}

		void DataFileUpdate(object sender, FileSystemEventArgs e) {
			var FileUri = new Uri(Path.GetFullPath(e.FullPath));
			var DataUri = new Uri(Path.GetFullPath(DATA_FOLDER_PATH));
			var RelativeUri = DataUri.MakeRelativeUri(FileUri);
			string SourcePath = FileUri.AbsolutePath;
			string DestinationPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/" + RelativeUri.OriginalString);
			File.Copy(SourcePath, DestinationPath, true);
			Console.WriteLine("Found change in " + Path.GetFileNameWithoutExtension(e.FullPath) + " - Copied to bin folder.");
			//ReloadLevel();
		}

		public override void Draw(GameTime gameTime) {
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "FPS: " + CorvusGame.Instance.FPS, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow);
			if(CorvusGame.Instance.Players.Any())
				CorvusGame.Instance.SpriteBatch.DrawString(Font, "Center: " + CorvusGame.Instance.Players.First().Character.Location.Center, new Vector2(10, GraphicsDevice.Viewport.Height - 50), Color.Yellow);
			base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime) {
			MouseState State = Mouse.GetState();
			if(State.LeftButton == ButtonState.Pressed && OldState.LeftButton != ButtonState.Pressed) {
				var Entity = SceneManager.ActiveScene.GetEntityAtPosition(new Point(State.X + (int)Player.Camera.Position.X, State.Y + (int)Player.Camera.Position.Y));
				if(Entity != null) {
					if(!CurrentCamera.IsDisposed)
						CurrentCamera.Dispose();
					CurrentCamera = new ChaseCameraComponent(Player.Camera);
					Entity.Components.Add(CurrentCamera);
				}
			}
			if(State.RightButton == ButtonState.Pressed && OldState.RightButton != ButtonState.Pressed) {
				var Entity = SceneManager.ActiveScene.GetEntityAtPosition(new Point(State.X + (int)Player.Camera.Position.X, State.Y + (int)Player.Camera.Position.Y));
				if(Entity != null) {
					foreach(var OtherEntity in SceneManager.ActiveScene.Entities.ToArray()) {
						if(OtherEntity != Entity && !CorvusGame.Instance.Players.Any(c => c.Character == OtherEntity))
							OtherEntity.Dispose();
					}
				}
			}
			base.Update(gameTime);
		}

		private void ReloadPressed(BindState State) {
			if(State != BindState.Pressed)
				return;
			ReloadLevel();
		}

		private void ClearCameraPressed(BindState State) {
			if(State != BindState.Pressed)
				return;
			if(!CurrentCamera.IsDisposed)
				CurrentCamera.Dispose();
			Player.Character.Components.Add((CurrentCamera = new ChaseCameraComponent(Player.Camera)));
		}

		private void ReloadLevel() {
			SceneManager.ReloadBlueprints();
			SceneManager.ReloadScenes();
		}

		private ChaseCameraComponent CurrentCamera;
		private MouseState OldState = Mouse.GetState();
		private SceneManager SceneManager;
		private CorvusPlayer Player;
	}
}
