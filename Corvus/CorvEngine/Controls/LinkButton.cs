using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    /// <summary>
    /// A selectable button.
    /// </summary>
    public class LinkButton : UIElement
    {
        private string _Text;
        private Color _SelectedColor = Color.Red;

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
        /// The color of the text when this is being focused.
        /// </summary>
        public Color SelectedColor
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; }
        }

        /// <summary>
        /// Creates a new instance LinkButton.
        /// </summary>
        public LinkButton()
            : base()
        {
            this.TabStop = true;
        }

        /// <summary>
        /// Updates the LinkButton.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (this.HasFocus)
                HandleInput();
        }

        /// <summary>
        /// Draws the LinkButton.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.HasFocus)
                spriteBatch.DrawString(this.SpriteFont, _Text, this.Position, _SelectedColor, 0f, new Vector2(), this.Scale, SpriteEffects.None, 0f);
            else
                spriteBatch.DrawString(this.SpriteFont, _Text, this.Position, this.Foreground, 0f, new Vector2(), this.Scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Handles input from the user.
        /// </summary>
        private void HandleInput() 
        {
            if (!this.HasFocus)
                return;

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                base.OnSelected();
        }

        /// <summary>
        /// Measures the size of the Linkbutton.
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
