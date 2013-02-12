using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    /// <summary>
    /// A class to wrap around a single element. 
    /// </summary>
    public class Border: Panel
    {
        private Texture2D _Background;
     
        /// <summary>
        /// The background image.
        /// </summary>
        public Texture2D Background
        {
            get { return _Background; }
            set { _Background = value; }
        }
        
        /// <summary>
        /// The control to wrap the border around.
        /// </summary>
        public UIElement Content
        {
            get { return this.Items[0]; }
            set 
            {
                if (this.Items.Count == 0)
                    this.Items.Add(value);
                else
                    this.Items[0] = value;

                value.ParentElement = this;
                MeasureSize(this.Items[0]);
            }
        }

        /// <summary>
        /// Creates a new instance of border.
        /// </summary>
        public Border()
            :base()
        {
            this.Items = new List<UIElement>(1);
        }

        /// <summary>
        /// Updates the border.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (Content != null)
                Content.Update(gameTime);
        }

        /// <summary>
        /// Draws the border and it's contents.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Content != null)
            {
                if (_Background != null)
                {
                    Rectangle rect = new Rectangle(0, 0, (int)this.Size.X, (int)this.Size.Y);
                    spriteBatch.Draw(_Background, this.Position, rect, Color.White);
                }

                Content.Position = this.Position;
                if(Content.IsVisible)
                    Content.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Measures the size of the border.
        /// </summary>
        /// <param name="element"></param>
        protected override void MeasureSize(UIElement element)
        {
            this.Size = Content.Size;
        }

        /// <summary>
        /// Not used in Border.
        /// </summary>
        public override void Add(UIElement element)
        {
            throw new NotImplementedException("Set directly to Content instead.");
        }

    }
}
