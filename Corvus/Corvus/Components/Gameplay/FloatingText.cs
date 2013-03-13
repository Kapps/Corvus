using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components.Gameplay
{
    /// <summary>
    /// A class to display the damage value.
    /// </summary>
    public class FloatingText
    {
        

        /// <summary>
        /// Gets or sets the value to display.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the Text size.
        /// </summary>
        public Vector2 TextSize { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Gets a value determining whether this text is finished animating.
        /// </summary>
        public bool IsFinished { get; private set; }

        private Vector2 _Position { get; set; }
        private TimeSpan _Timer = new TimeSpan();
        private TimeSpan _Duration = TimeSpan.FromMilliseconds(550);
        private float _YIncrement = 0f;

        /// <summary>
        /// Creates a new instance of FloatingText.
        /// </summary>
        public FloatingText(string value, Vector2 textSize, Color textColor)
        {
            Value = value;
            TextSize = textSize;
            TextColor = textColor;
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            _Position = position + new Vector2(0, _YIncrement) - new Vector2(TextSize.X / 2, 0); //to center and make it move up
            _YIncrement -= 0.075f;
            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= _Duration)
                IsFinished = true;
        }

        public void Draw(SpriteFont font)
        {
            //TODO: Might want to make the font scale with the size of the object.
            CorvusGame.Instance.SpriteBatch.DrawString(font, Value, _Position, TextColor);
        }
    }
}
