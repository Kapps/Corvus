using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    /// <summary>
    /// The base class for interface elements.
    /// </summary>
    public abstract class UIElement
    {
        #region Fields/Properties

        private string _Name;
        private Vector2 _Size;
        private Vector2 _Position;
        private bool _HasFocus;
        private bool _IsVisible;
        private bool _IsEnabled;
        private bool _TabStop;
        private Color _Foreground;
        private float _Scale = 1f;
        private SpriteFont _SpriteFont;
        private UIElement _ParentElement = null;

        /// <summary>
        /// Used to identify this element. Is not case sensitive.
        /// </summary>
        public virtual string Name
        {
            get { return _Name.ToLower(); }
            set { _Name = value.ToLower(); }
        }

        /// <summary>
        /// Returns the unscaled size of the element.
        /// </summary>
        public virtual Vector2 Size
        {
            get { return _Size; } 
            protected set { _Size = value ; } 
        }

        /// <summary>
        /// Gets or sets the position of the element.
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
        public virtual Color Foreground
        {
            get { return _Foreground; }
            set { _Foreground = value; }
        }

        /// <summary>
        /// Gets or sets a value to scale the element by.
        /// </summary>
        public virtual float Scale
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
            set { _SpriteFont = value; }
        }

        /// <summary>
        /// Gets the parent element, if any.
        /// </summary>
        public UIElement ParentElement
        {
            get { return _ParentElement; }
            internal set { _ParentElement = value; }
        }

        /// <summary>
        /// Gets the scaled size of this element.
        /// </summary>
        public Vector2 ScaledSize { get { return _Size * _Scale; } }

        #endregion

        #region Events
        /// <summary>
        /// Event to handle if this element is selected.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Calls the selected event if it is not null.
        /// </summary>
        protected virtual void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of UIElement.
        /// </summary>
        public UIElement()
        {
            this.Name = string.Empty;
            this._Position = new Vector2();
            this._Size = new Vector2();
            this._IsVisible = true;
            this._IsEnabled = true;
            this._Foreground = Color.Black;
            this._HasFocus = false;
        }

        /// <summary>
        /// Updates the element.
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Draws the element
        /// </summary>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Determines the unscaled size of the element.
        /// </summary>
        protected abstract void MeasureSize();

    }
}
