using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls.UserControls
{
    public class Menu : UserControl
    {
        private StackPanel _StackPanel = new StackPanel();

        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                _StackPanel.IsEnabled = value;
            }
        }

        public Menu(SpriteFont spriteFont)
        {
            this.SpriteFont = spriteFont;
            this.Content = _StackPanel;
            this.Foreground = Color.Black;
        }

        public Menu(SpriteFont spriteFont, Color foreground)
            :this(spriteFont)
        {
            this.Foreground = foreground;
        }

        public void AddItem(string text, EventHandler selected)
        {
            _StackPanel.Add(new MenuItem(this.SpriteFont, text, selected, this.Foreground));
            this.Content = _StackPanel; //need to resize it.
        }

        public LinkButton GetButton(int index)
        {
            if (_StackPanel.Items.Count != 0)
                return ((MenuItem)_StackPanel.Items[(int)MathHelper.Clamp(index, 0, _StackPanel.Items.Count)]).Button;
            else
                return null;
        }
    }
}
