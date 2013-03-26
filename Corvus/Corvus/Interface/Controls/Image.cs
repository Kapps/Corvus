using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;


namespace Corvus.Interface.Controls
{
    /// <summary>
    /// A class that draws a image control.
    /// </summary>
    public class Image : BaseControl
    {
        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        public Texture2D Texture { get; set; }

        public Image(Texture2D texture)
            : base(null)
        {
            Texture = texture;
            Size = new Vector2(Texture.Width, Texture.Height);
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Draw(Texture, Position, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), Color);
        }
    }
}
