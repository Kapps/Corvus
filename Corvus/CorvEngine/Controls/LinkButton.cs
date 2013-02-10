using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    //Need to implement measure size
    public class LinkButton : UIElement
    {
        private string _Text;
        private Color _SelectedColor = Color.Red;

        public string Text
        {
            get { return _Text; }
            set 
            { 
                _Text = value;
                MeasureSize();
            }
        }

        public Color SelectedColor
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; }
        }

        public LinkButton()
            : base()
        {
            this.TabStop = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.HasFocus)
                HandleInput();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.HasFocus)
                spriteBatch.DrawString(this.SpriteFont, _Text, this.Position, _SelectedColor);
            else
                spriteBatch.DrawString(this.SpriteFont, _Text, this.Position, this.Foreground);
        }

        private void HandleInput() 
        {
            if (!this.HasFocus)
                return;

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                base.OnSelected();
        }

        protected override void MeasureSize()
        {
            if (!string.IsNullOrEmpty(_Text))
                this.Size = this.SpriteFont.MeasureString(_Text);
            else
                this.Size = new Vector2();   
        }

    }
}
