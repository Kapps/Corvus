using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    public class Image : UIElement
    {
        private Texture2D _Source = null;

        public Texture2D Source
        {
            get { return _Source; }
            set
            { 
                _Source = value;
                MeasureSize();
            }
        }

        public Image()
            :base()
        {
            this.TabStop = false;
            this.Foreground = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_Source, this.Position, this.Source.Bounds, this.Foreground, 0f, new Vector2(), this.Scale, SpriteEffects.None, 0f);
        }
        
        protected override void MeasureSize()
        {
            if (_Source != null)
                this.Size = new Vector2(this._Source.Width, this._Source.Bounds.Height);
            else
                this.Size = new Vector2();
        }
    }
}
