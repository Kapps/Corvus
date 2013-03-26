using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;

namespace Corvus.Interface.Controls
{
    /// <summary>
    /// A class to draw text.
    /// </summary>
    public class Label : BaseControl
    {
        private string _Text = "";

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        public string Text 
        {
            get { return _Text; }
            set 
            {
                if (_Text != value)
                {
                    _Text = value;
                    Size = SpriteFont.MeasureString(_Text);
                }
            }
        }

        public Label(SpriteFont font)
            : base(font)
        {
        }

        public override void Update(GameTime gameTime)
        {
                   
        }

        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.DrawString(SpriteFont, Text, Position, Color, 0f, new Vector2(), Scale, SpriteEffects.None, 0f);
        }

    }
}
