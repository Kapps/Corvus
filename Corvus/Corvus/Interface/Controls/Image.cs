using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;


namespace Corvus.Interface.Controls
{
    public class Image : BaseControl
    {
        private Texture2D Texture { get; set; }

        public Image(Texture2D texture)
            : base(null)
        {
            Texture = texture;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Draw(Texture, Position, Texture.Bounds, Color);
        }
    }
}
