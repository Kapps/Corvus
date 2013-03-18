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
        private Random _Rand = new Random();
        private List<FloatingText> _ActiveList = new List<FloatingText>();
        private TimeSpan _DelayTimer = TimeSpan.Zero;

        /// <summary>
        /// Adds a floating text with the specified value and color.
        /// </summary>
        public void AddFloatingTexts(float value, Color color)
        {
            string text = Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString();
            this.Add(new FloatingText(text, _Font.MeasureString(text), color));
        }

        /// <summary>
        /// Adds a floating text with the specified string and color.
        /// </summary>
        public void AddFloatingTexts(string value, Color color)
        {
            this.Add(new FloatingText(value, _Font.MeasureString(value), color));
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            foreach (FloatingText dt in _ActiveList.Reverse<FloatingText>())
            {
                dt.Update(gameTime, position);
                if (dt.IsFinished)
                    _ActiveList.Remove(dt);
            }

            if (this.Count == 0)
                return;
            _DelayTimer += gameTime.ElapsedGameTime;
            if (_ActiveList.Count == 0 || _DelayTimer >= TimeSpan.FromMilliseconds(300))
            {
                var item = this.First();
                _ActiveList.Add(item);
                this.Remove(item);
                _DelayTimer = TimeSpan.Zero;
            }
        }

        public void Draw()
        {
            foreach (FloatingText dt in _ActiveList)
                dt.Draw(_Font);
        }
    }
}
