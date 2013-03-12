using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;

namespace CorvEngine.Components {
	/// <summary>
	/// Provides a Component used to attach a Sprite to an Entity.
	/// </summary>
	[Serializable]
	public class SpriteComponent : Component {
		private Sprite _Sprite;
		private Color _Color = Color.White;

		public override string Name {
			get {
				return "Sprite Component";
			}
		}

		/// <summary>
		/// Gets or sets the color tint to apply to the sprite being used for this component.
		/// </summary>
		public Color Color {
			get { return _Color; }
			set { _Color = value; }
		}

		/// <summary>
		/// Gets or sets the Sprite being used to render this component.
		/// </summary>
		public Sprite Sprite {
			get { return _Sprite; }
			set { _Sprite = value; }
		}

		// For serialization.
		private SpriteComponent() { }

		/// <summary>
		/// Creates a new SpriteComponent with the specified Sprite set.
		/// </summary>
		public SpriteComponent(Sprite Sprite) {
			this._Sprite = Sprite;
		}

		protected override void OnDraw() {
			base.OnDraw();
			var SpriteBatch = CorvBase.Instance.SpriteBatch;
			Vector2 ScreenPosition = Camera.Active.ScreenToWorld(Parent.Position);
			var ActiveFrame = Sprite.ActiveAnimation.ActiveFrame.Frame;
			var SourceRect = ActiveFrame.Source;
			SpriteBatch.Draw(this.Sprite.Texture, new Rectangle((int)ScreenPosition.X, (int)ScreenPosition.Y, (int)Parent.Size.X, (int)Parent.Size.Y), SourceRect, Color);
		}

		protected override void OnUpdate(GameTime Time) {
			base.OnUpdate(Time);
			Sprite.ActiveAnimation.AdvanceAnimation(Time.ElapsedGameTime);
		}
	}
}
