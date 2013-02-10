using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Controls;

namespace CorvEngine.Controls.UserControls
{
    public class MessageBox : UserControl
    {
        private const int MAX_TEXT_LENGTH = 50;

        private Border _Border;
        private StackPanel _StackPanel;
        private TextBlock _TextBlock;
        private LinkButton _Button;

        public new event EventHandler Selected;

        public LinkButton Button { get { return _Button; } }

        public override bool HasFocus
        {
            get { return _Button.HasFocus; }
            set { _Button.HasFocus = value; }
        }

        public override bool IsVisible
        {
            get { return base.IsVisible; }
            set
            {
                base.IsVisible = value;
                base.IsEnabled = value;
            }
        }

        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                base.IsVisible = value;
            }
        }

        public override Vector2 Position
        {
            get { return _Border.Position; }
            set { _Border.Position = value; }
        }

        public MessageBox(string message, SpriteFont spriteFont, Texture2D _background)
            :base()
        {
            this.IsVisible = false;

            this._Border = new Border();
            this._Border.Background = _background;
            
            this._StackPanel = new StackPanel();

            this._TextBlock = new TextBlock();
            this._TextBlock.Length = MAX_TEXT_LENGTH;
            this._TextBlock.SpriteFont = spriteFont;
            this._TextBlock.Text = message;
            this._TextBlock.Foreground = Color.AliceBlue;
            this._StackPanel.Add(this._TextBlock);

            this._Button = new LinkButton();
            this._Button.SpriteFont = spriteFont;
            this._Button.Text = "Continue";
            this._Button.Selected += _Button_Selected;
            this._StackPanel.Add(this._Button);

            this._Border.Content = this._StackPanel;
            this.Content = this._Border;
        }

       
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.IsVisible)
                base.Draw(spriteBatch);
        }

        public void Show()
        {
            this.IsVisible = true;
        }

        public void Hide()
        {
            this.IsVisible = false;
        }

        void _Button_Selected(object sender, EventArgs e)
        {
            if(Selected != null)
                Selected(this, new EventArgs());
        }
        
    }
}
