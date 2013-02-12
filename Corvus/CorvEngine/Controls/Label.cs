using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    /// <summary>
    /// A Control to display a text label.
    /// </summary>
    public class Label : UIElement
    { 
        private string _Text = "";

        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set 
            { 
                _Text = value;

                MeasureSize();
            }
        }

        /// <summary>
        /// Creates an instance of Label.
        /// </summary>
        public Label()
            :base()
        {
            this.TabStop = false;
        }

        /// <summary>
        /// Update the label.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.SpriteFont, _Text, this.Position, this.Foreground, 0f, new Vector2(), this.Scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Measures the size of the label.
        /// </summary>
        protected override void MeasureSize()
        {
            if (!string.IsNullOrEmpty(_Text))
                this.Size = this.SpriteFont.MeasureString(_Text);
            else
                this.Size = new Vector2();
        }
    }
}
