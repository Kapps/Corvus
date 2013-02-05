using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    public class ControlManager : List<UIElement>
    {
        private bool _AcceptsInput = true;

        public bool AcceptsInput
        {
            get { return _AcceptsInput; }
            set { _AcceptsInput = value; }
        }

        public ControlManager()
            :base()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
