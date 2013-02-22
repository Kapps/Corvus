using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components {
	/// <summary>
	/// Handles the damage events for this entity.
	/// </summary>
	public class DamageComponent : Component {
		private SpriteFont _Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/DamageFont");
		private List<DamageText> _DamageTexts = new List<DamageText>();

		/// <summary>
		/// Applies damage with the normal rules.
		/// </summary>
		public void TakeDamage(float incomingDamage) {
			var ac = this.GetDependency<AttributesComponent>();
			float damageTaken = NormalDamageFormula(ac.Defense, incomingDamage);
			ac.CurrentHealth -= damageTaken;

			string text = ((int)damageTaken).ToString();
			_DamageTexts.Add(new DamageText(text, _Font.MeasureString(text)));
		}

		private float NormalDamageFormula(float myDefense, float incomingDamage) {
			return Math.Max(incomingDamage - (myDefense * 0.70f), 1);
		}

		protected override void OnUpdate(GameTime Time) {
			base.OnUpdate(Time);
			foreach(DamageText dt in _DamageTexts.Reverse<DamageText>()) {
				var width = Parent.Size.X;
				var position = Parent.Position + new Vector2(width / 2, 0);
				var ToScreen = Camera.Active.ScreenToWorld(position);
				dt.Update(Time, ToScreen);
				if(dt.IsFinished)
					_DamageTexts.Remove(dt);
			}
		}

		protected override void OnDraw() {
			base.OnDraw();
			foreach(DamageText dt in _DamageTexts)
				dt.Draw(_Font);
		}

		/// <summary>
		/// A class to display the damage value.
		/// </summary>
		private class DamageText {

			// TODO: This should be in it's own class so that other components can use it.

			/// <summary>
			/// Gets or sets the value to display.
			/// </summary>
			public string Value { get; set; }

			/// <summary>
			/// Gets or sets the Text size.
			/// </summary>
			public Vector2 TextSize { get; set; }

			/// <summary>
			/// Gets a value determining whether this text is finished animating.
			/// </summary>
			public bool IsFinished { get; private set; }

			private Vector2 _Position { get; set; }
			private TimeSpan _Timer = new TimeSpan();
			private TimeSpan _Duration = TimeSpan.FromMilliseconds(450);
			private float _YIncrement = 0f;

			/// <summary>
			/// Creates a new instance of DamageText.
			/// </summary>
			public DamageText(string value, Vector2 textSize) {
				Value = value;
				TextSize = textSize;
			}

			public void Update(GameTime gameTime, Vector2 position) {
				_Position = position + new Vector2(0, _YIncrement) - new Vector2(TextSize.X / 2, 0); //to center and make it move up
				_YIncrement -= 0.075f;
				_Timer += gameTime.ElapsedGameTime;
				if(_Timer >= _Duration)
					IsFinished = true;
			}

			public void Draw(SpriteFont font) {
				//TODO: Might want to make the font scale with the size of the object.
				CorvusGame.Instance.SpriteBatch.DrawString(font, Value, _Position, Color.White);
			}
		}

	}
}
