using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;

namespace Corvus.Interface.Controls
{
    public class LinkButton : BaseControl
    {
        private string _Text = "";
        private Color _FocusedColor = Color.Red;
        private Vector2 _Size = new Vector2();
        
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    _Size = SpriteFont.MeasureString(_Text);
                }
            }
        }

        public Color FocusedColor
        {
            get { return _FocusedColor; }
            set { _FocusedColor = value; }
        }

        public Vector2 Size { get { return _Size; } }

        public LinkButton(SpriteFont font)
            : base(font)
        {
            this.TabStop = true;
            this.HasFocus = false;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.HasFocus)
                this.SpriteBatch.DrawString(SpriteFont, Text, Position, FocusedColor, 0f, new Vector2(), Scale, SpriteEffects.None, 0f);
            else
                this.SpriteBatch.DrawString(SpriteFont, Text, Position, Color, 0f, new Vector2(), Scale, SpriteEffects.None, 0f);
        }
    }
}
