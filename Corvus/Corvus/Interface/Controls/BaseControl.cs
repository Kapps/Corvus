using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;

namespace Corvus.Interface.Controls
{
    public abstract class BaseControl
    {
        /// <summary>
        /// Event to handle if this element is selected.
        /// </summary>
        public event EventHandler Selected;

        private Vector2 _Position = new Vector2();
        private bool _HasFocus = false;
        private bool _IsVisible = true;
        private bool _IsEnabled = true;
        private bool _TabStop = false;
        private Color _Color = Color.White;
        private float _Scale = 1f;
        private SpriteFont _SpriteFont;

        /// <summary>
        /// Gets the sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatch { get { return CorvBase.Instance.SpriteBatch; } }

        /// <summary>
        /// Gets or sets the position of the element, with the margin applied. 
        /// </summary>
        public virtual Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if this element has focus.
        /// </summary>
        public virtual bool HasFocus
        {
            get { return _HasFocus; }
            set { _HasFocus = value; }
        }

        /// <summary>
        /// Gets or sets the visibility of the element.
        /// </summary>
        public virtual bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if this element is enabled.
        /// </summary>
        public virtual bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; }
        }

        /// <summary>
        /// Gets a value indicating that his element is focusable.
        /// </summary>
        public virtual bool TabStop
        {
            get { return _TabStop; }
            protected set { _TabStop = value; }
        }

        /// <summary>
        /// Gets or sets the color of the foreground content.
        /// </summary>
        public virtual Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating how much to scale this control by.
        /// </summary>
        public float Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        /// <summary>
        /// Gets or sets the spritefont.
        /// </summary>
        public SpriteFont SpriteFont
        {
            get { return _SpriteFont; }
            private set { _SpriteFont = value; }
        }

        /// <summary>
        /// Calls the selected event if it is not null.
        /// </summary>
        public void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        /// <summary>
        /// Creates a new control with the specified font.
        /// </summary>
        public BaseControl(SpriteFont font)
        {
            this.SpriteFont = font;
        }

        /// <summary>
        /// Updates the element.
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Draws the element
        /// </summary>
        public abstract void Draw(GameTime gameTime);

    }
}
