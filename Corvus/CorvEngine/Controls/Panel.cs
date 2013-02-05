using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    public abstract class Panel : UIElement
    {
        private List<UIElement> _Items;
        protected virtual List<UIElement> Items 
        { 
            get { return _Items; }
            set { _Items = value; }
        }

        public Panel()
        {
            _Items = new List<UIElement>();
        }
    }
}
