using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Entities;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus {
	public class TestState : GameState {
		public override bool BlocksUpdate {
			get { return true; }
		}

		public override bool BlocksDraw {
			get { return true; }
		}

		public static Scene Scene;

		public TestState() {
			foreach(var BlueprintFile in Directory.GetFiles("Data/Entities", "*.txt"))
				BlueprintParser.ParseBlueprint(File.ReadAllText(BlueprintFile));
			Scene = new Scene(LevelData.LoadTmx("Data/Levels/BasicLevel.tmx"), this);
			this.AddComponent(Scene);
			this.AddComponent(new TestComponent(this));
		}

		private class TestComponent : GameStateComponent {

			private SpriteBatch Batch = new SpriteBatch(CorvBase.Instance.GraphicsDevice);
			private static Random rnd = new Random();
			private Color Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
			private Texture2D Texture;
			private MovementComponent mc;

			public TestComponent(GameState State) : base(State) {
				var Loader = new ContentManager(CorvBase.Instance.Services, "Content");
				this.Texture = Loader.Load<Texture2D>("TestTexture");

				var Blueprint = EntityBlueprint.GetBlueprint("TestEntity");
				var PlayerEntity = Blueprint.CreateEntity();
				// This stuff is obviously things that the ctor should handle.
				// And things like size should probably be dependent upon the actual animation being played.
				PlayerEntity.Size = new Vector2(40, 32);
				PlayerEntity.Position = new Vector2(PlayerEntity.Location.Width, 10);
				PlayerEntity.Velocity = new Vector2(0, 0);
				mc = PlayerEntity.GetComponent<MovementComponent>();
				mc.isGrounded = false;
				var Player = new CorvusPlayer(PlayerEntity);
				PlayerEntity.Components.Add(new ChaseCameraComponent(Player.Camera));
				CorvusGame.Instance.AddPlayer(Player);
				CorvusBinds.CreateBinds(Player);
				Scene.AddEntity(Player.Character);
			}

			protected override void OnUpdate(GameTime Time) {
				mc.ApplyPhysics(Time, Scene);
			}

			protected override void OnDraw(GameTime Time) {
				//throw new NotImplementedException();
			}
		}
	}
}
