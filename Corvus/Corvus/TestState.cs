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

		public Scene Scene;

		public TestState() {
			foreach(var BlueprintFile in Directory.GetFiles("Data/Entities", "*.txt"))
				BlueprintParser.ParseBlueprint(File.ReadAllText(BlueprintFile));
			this.Scene = new Scene(LevelData.LoadTmx("Data/Levels/BasicLevel.tmx"), this);
			this.AddComponent(Scene);
			this.AddComponent(new TestComponent(this));
		}

		private class TestComponent : GameStateComponent {

			private SpriteBatch Batch = new SpriteBatch(CorvBase.Instance.GraphicsDevice);
			private static Random rnd = new Random();
			private Color Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
			private Texture2D Texture;
			private TempPlayer player;

			public TestComponent(GameState State) : base(State) {
				var Loader = new ContentManager(CorvBase.Instance.Services, "Content");
				this.Texture = Loader.Load<Texture2D>("TestTexture");
				player = new TempPlayer(((TestState)State).Scene);
			}

			protected override void OnDraw(GameTime Time) {
				//Batch.Begin();
				//Batch.Draw(Texture, new Rectangle(0, 0, 800, 600), Color);
				//Batch.End();
                player.Draw(); //Player should be drawn last, providing nothing overlaps them.
			}

			protected override void OnUpdate(GameTime Time) {
				player.Update(Time);
			}
		}
	}
}
