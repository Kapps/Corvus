using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components.Gameplay
{
    /// <summary>
    /// A collection of FloatingText. 
    /// </summary>
    public class FloatingTextList : List<FloatingText>
    {
        private SpriteFont _Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/DamageFont");

        /// <summary>
        /// Adds a floating text with the specified value and color.
        /// </summary>
        public void AddFloatingTexts(float value, Color color)
        {
            string text = Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString();
            this.Add(new FloatingText(text, _Font.MeasureString(text), color));
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            foreach (FloatingText dt in this.Reverse<FloatingText>())
            {
                dt.Update(gameTime, position);
                if (dt.IsFinished)
                    this.Remove(dt);
            }
        }

        public void Draw()
        {
            foreach (FloatingText dt in this)
                dt.Draw(_Font);
        }
    }
}
