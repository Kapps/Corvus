using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls.UserControls
{
    public class MenuItem : UserControl
    {
        private LinkButton _Button;

        public LinkButton Button { get { return _Button; } }

        public override bool IsEnabled
        {
            get { return _Button.IsEnabled; }
            set { _Button.IsEnabled = value; }
        }

        public override bool HasFocus
        {
            get { return _Button.HasFocus; }
            set { _Button.HasFocus = value; }
        }

        public MenuItem(SpriteFont spriteFont, string text, EventHandler selected)
        {
            this._Button = new LinkButton();
            this._Button.SpriteFont = spriteFont;
            this._Button.Text = text;
            this._Button.Selected += selected;

            this.Content = _Button;
        }

        public MenuItem(SpriteFont spriteFont, string text, EventHandler selected, Color foreground)
            : this(spriteFont, text, selected)
        {
            this._Button.Foreground = foreground;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if(this.IsVisible)
                base.Draw(spriteBatch);
        }
    }
}
