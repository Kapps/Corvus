using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Controls;

namespace CorvEngine.Controls.UserControls
{
    /// <summary>
    /// A control to display a MessageBox.
    /// </summary>
    public class MessageBox : UserControl
    {
        private const int MAX_TEXT_LENGTH = 50;

        private Border _Border;
        private StackPanel _StackPanel;
        private TextBlock _TextBlock;
        private LinkButton _Button;

        /// <summary>
        /// Fires when the continue button is pressed.
        /// </summary>
        public new event EventHandler Selected;

        /// <summary>
        /// Gets the LinkButton.
        /// </summary>
        public LinkButton Button { get { return _Button; } }

        /// <summary>
        /// Gets or sets a value indicating that the LinkButton has focus.
        /// </summary>
        public override bool HasFocus
        {
            get { return _Button.HasFocus; }
            set { _Button.HasFocus = value; }
        }

        /// <summary>
        /// Gets or sets the visibility of the MessageBox. Will also set the value of IsEnabled.
        /// </summary>
        public override bool IsVisible
        {
            get { return base.IsVisible; }
            set
            {
                base.IsVisible = value;
                base.IsEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this MessageBox is enabled. Will also set the value of IsVisible.
        /// </summary>
        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                base.IsVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the MessageBox.
        /// </summary>
        public override Vector2 Position
        {
            get { return _Border.Position; }
            set { _Border.Position = value; }
        }

        /// <summary>
        /// Creates a new instance of MessageBox.
        /// </summary>
        /// <param name="message">The text to display.</param>
        /// <param name="spriteFont">The spritefont to use.</param>
        /// <param name="_background">The background image to use.</param>
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

        /// <summary>
        /// Draws the MessageBox.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.IsVisible)
                base.Draw(spriteBatch);
        }

        /// <summary>
        /// Shows the Messagebox.
        /// </summary>
        public void Show()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// Hides the MessageBox.
        /// </summary>
        public void Hide()
        {
            this.IsVisible = false;
        }

        /// <summary>
        /// Fires if the Selected value isn't null and the continue button is selected.
        /// </summary>
        void _Button_Selected(object sender, EventArgs e)
        {
            if(Selected != null)
                Selected(this, new EventArgs());
        }
        
    }
}
