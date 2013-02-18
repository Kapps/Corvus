using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a component for an Entity that can take damage and die.
	/// </summary>
	public class HealthComponent : Component {
		/// <summary>
		/// Gets an event called when the health of this Component runs out.
		/// </summary>
		public event Action<HealthComponent> Died;
		/// <summary>
		/// Gets an event called when the amount of health this Component has remaining is changed.
		/// </summary>
		public event Action<HealthComponent> CurrentHealthChanged;
		/// <summary>
		/// Gets an event called when the max health this Component has is changed.
		/// </summary>
		public event Action<HealthComponent> MaxHealthChanged;

		private static Texture2D HealthBarTexture = CorvBase.Instance.GlobalContent.Load<Texture2D>("Interface/HealthBar");

		/// <summary>
		/// Gets or sets the amount of health that this component has.
		/// </summary>
		public float CurrentHealth {
			get { return _CurrentHealth; }
			set {
				value = Math.Min(MaxHealth, Math.Max(value, 0));
				_CurrentHealth = value;
				if(this.CurrentHealthChanged != null)
					CurrentHealthChanged(this);
			}
		}

		/// <summary>
		/// Gets or sets the maximum health allowed for this component.
		/// Setting this value also alters the current health to be equal to the old percentage of health.
		/// </summary>
		public float MaxHealth {
			get { return _MaxHealth; }
			set {
				float CurrPercent = CurrentHealth / MaxHealth;
				_MaxHealth = value;
				if(MaxHealthChanged != null)
					MaxHealthChanged(this);
				CurrentHealth = MaxHealth * CurrPercent;
			}
		}

		public override void Draw() {
			var Width = Parent.Size.X;
			var Height = Parent.Size.X * 0.20f;
			var Location = Parent.Position - new Vector2(0, Height + 5);
			var ToScreen = Camera.Active.ScreenToWorld(Location);
			Color HealthColor = Color.Lerp(Color.Red, Color.Green, CurrentHealth / MaxHealth);
			//HealthColor = new Color(HealthColor.R, HealthColor.G, HealthColor.B, 40); // Give some transparency.
			CorvBase.Instance.SpriteBatch.Draw(HealthBarTexture, new Rectangle((int)ToScreen.X, (int)ToScreen.Y, (int)Width, (int)Height), HealthColor);
			base.Draw();
		}

		private float _CurrentHealth = 100;
		private float _MaxHealth = 100;
	}
}
