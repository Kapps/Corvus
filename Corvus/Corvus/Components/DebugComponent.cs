using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CorvEngine;
using CorvEngine.Geometry;
using CorvEngine.Graphics;
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
			Bind ToggleGeometryBind = new Bind(Player.InputManager, ToggleGeometryPressed, false, Keys.F11);
			Player.InputManager.RegisterBind(ToggleGeometryBind);
			Bind ToggleEntityBind = new Bind(Player.InputManager, ToggleEntityPressed, false, Keys.F10);
			Player.InputManager.RegisterBind(ToggleEntityBind);
			CurrentCamera = Player.Character.GetComponent<ChaseCameraComponent>();
			GeometryTexture = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/Outline");
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
			if(DisplayGeometry) {
				TiledPlatformerGeometry Geometry = (TiledPlatformerGeometry)CorvusGame.Instance.SceneManager.ActiveScene.Geometry;
				foreach(TiledPlatformerGeometryObject GeometryObj in Geometry.GeometryObjects) {
					CorvusGame.Instance.SpriteBatch.Draw(GeometryTexture, Camera.Active.WorldToScreen(GeometryObj.Location), new Color(255, 0, 0, 64));
				}
			}
			if(DisplayEntities) {
				foreach(var Entity in SceneManager.ActiveScene.Entities) {
					CorvusGame.Instance.SpriteBatch.Draw(GeometryTexture, Camera.Active.WorldToScreen(Entity.Location), new Color(0, 0, 255, 64));
				}
			}
			CorvusGame.Instance.SpriteBatch.DrawString(Font, "FPS: " + CorvusGame.Instance.FPS, new Vector2(10, GraphicsDevice.Viewport.Height - 30), Color.Yellow);
			if(CorvusGame.Instance.Players.Any())
				CorvusGame.Instance.SpriteBatch.DrawString(Font, "Center: " + CorvusGame.Instance.Players.First().Character.Location.Center, new Vector2(10, GraphicsDevice.Viewport.Height - 50), Color.Yellow);
			string GenText = "Garbage Collections Per Generation - ";
			for(int i = 0; i <= GC.MaxGeneration; i++)
				GenText += "{" + i + ", " + GC.CollectionCount(i) + "} ";
			CorvusGame.Instance.SpriteBatch.DrawString(Font, GenText, new Vector2(10, GraphicsDevice.Viewport.Height - 70), Color.Yellow);
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

		private void ToggleGeometryPressed(BindState State) {
			if(State != BindState.Pressed)
				return;
			DisplayGeometry = !DisplayGeometry;
		}

		private void ToggleEntityPressed(BindState State) {
			if(State != BindState.Pressed)
				return;
			DisplayEntities = !DisplayEntities;
		}

		private void ReloadLevel() {
			SceneManager.ReloadBlueprints();
			SceneManager.ReloadScenes();
		}

		private Color GenerateRandomColorForObject(object obj, int alpha) {
			if(RandomObjectColors.ContainsKey(obj))
				return RandomObjectColors[obj];
			Color col = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), alpha / 255f);
			RandomObjectColors[obj] = col;
			return col;
		}

		private ChaseCameraComponent CurrentCamera;
		private MouseState OldState = Mouse.GetState();
		private SceneManager SceneManager;
		private CorvusPlayer Player;
		private bool DisplayGeometry;
		private bool DisplayEntities;
		private Texture2D GeometryTexture;
		private Dictionary<object, Color> RandomObjectColors = new Dictionary<object, Color>();
		private Random rnd = new Random();
	}
}
