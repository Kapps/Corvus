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
    /// A base class for custom controls. These usually contain many of the basic UIElements.
    /// </summary>
    public abstract class UserControl : UIElement
    {
        private UIElement _Content = null;

        /// <summary>
        /// Gets or sets the content of the UserControl.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public override Vector2 Position
        {
            get { return this._Content.Position; }
            set { this._Content.Position = value; }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        public override Thickness Margin
        {
            get { return _Content.Margin; }
            set { _Content.Margin = value; }
        }

        /// <summary>
        /// Updates the UserControl.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (_Content != null)
                _Content.Update(gameTime);
        }

        /// <summary>
        /// Draws the UserControl.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_Content != null)
                _Content.Draw(spriteBatch);
        }

        /// <summary>
        /// Measures the size of the UserControl.
        /// </summary>
        protected override void MeasureSize()
        {
            if (_Content == null)
                this.Size = new Vector2();

            this.Size = _Content.ScaledSize;
        }

    }
}
