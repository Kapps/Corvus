using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components {
	/// <summary>
	/// Provides a component for an Entity that can take damage and die.
	/// </summary>
	public class HealthBarComponent : Component {
        private static Texture2D HealthBarTexture = CorvBase.Instance.GlobalContent.Load<Texture2D>("Interface/HealthBar");
        private static Texture2D HealthMeterTexture = CorvBase.Instance.GlobalContent.Load<Texture2D>("Interface/HealthMeter");

		protected override void OnDraw() {
            Entity entity = this.Parent;
            var ac = entity.GetComponent<AttributesComponent>();
            if (ac.CurrentHealth / ac.MaxHealth == 1)
                return; //Healthy, no need to draw

			var Width = Parent.Size.X;
			var Height = Parent.Size.X * 0.15f;
			var Location = Parent.Position - new Vector2(0, Height + 5);
			var ToScreen = Camera.Active.WorldToScreen(Location);
			Color HealthColor = Color.Lerp(Color.Red, Color.Green, ac.CurrentHealth / ac.MaxHealth);
			//HealthColor = new Color(HealthColor.R, HealthColor.G, HealthColor.B, 40); // Give some transparency.

            //Draw grey area
            CorvBase.Instance.SpriteBatch.Draw(HealthBarTexture, new Rectangle((int)ToScreen.X, (int)ToScreen.Y, (int)Width, (int)Height),
                                            new Rectangle(0, 0, HealthBarTexture.Width, HealthBarTexture.Height), Color.Gray);
            //Draw actual health
            CorvBase.Instance.SpriteBatch.Draw(HealthMeterTexture, new Rectangle((int)ToScreen.X, (int)ToScreen.Y, (int)(Width * (ac.CurrentHealth / ac.MaxHealth)), (int)Height),
                                            new Rectangle(0, 0, HealthMeterTexture.Width, HealthMeterTexture.Height), HealthColor);

			base.OnDraw();
		}
	}
}
