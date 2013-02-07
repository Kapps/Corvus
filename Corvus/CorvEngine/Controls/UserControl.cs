using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Controls;

//UserControl should implement HasFocus, IsVisible, IsEnabled.
namespace CorvEngine.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UserControl : UIElement
    {
        private UIElement _Content = null;

        public virtual UIElement Content
        {
            get { return _Content; }
            set 
            {
                _Content = value;

                value.ParentElement = this;
                MeasureSize();
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            if (_Content != null)
                _Content.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_Content != null)
                _Content.Draw(spriteBatch);
        }

        protected override void MeasureSize()
        {
            if (_Content == null)
                this.Size = new Vector2();

            this.Size = _Content.Size;
        }

    }
}
