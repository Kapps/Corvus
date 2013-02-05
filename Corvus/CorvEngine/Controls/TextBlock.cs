using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CorvEngine.Controls
{
    public class TextBlock : UIElement
    {
        private string _Text = "";

        public string Text
        {
            get { return _Text; }
            set 
            { 
                _Text = value;
                MeasureSize();
            }
        }

        public TextBlock()
            :base()
        {
            this.TabStop = false;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.SpriteFont, _Text, this.Position, this.Foreground, 0f, new Vector2(), this.Scale, SpriteEffects.None, 0f);
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
