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

        public virtual List<UIElement> Items 
        { 
            get { return _Items; }
            protected set { _Items = value; }
        }

        public override bool HasFocus
        {
            get { return base.HasFocus; }
            set { base.HasFocus = value; }
        }

        /// <summary>
        /// Enables all elements within this panel.
        /// </summary>
        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                base.IsEnabled = value;
                foreach (UIElement e in this.Items)
                    e.IsEnabled = value;
            }
        }

        /// <summary>
        /// Sets visibility for all elements within this panel.
        /// </summary>
        public override bool IsVisible
        {
            get { return base.IsVisible; }
            set
            {
                base.IsVisible = value;
                foreach (UIElement e in this.Items)
                    e.IsVisible = value;
            }
        }

        /// <summary>
        /// Scales all the elements in the StackPanel.
        /// </summary>
        public override float Scale
        {
            get { return base.Scale; }
            set
            {
                base.Scale = value;
                foreach (UIElement element in this.Items)
                    element.Scale *= base.Scale;
            }
        }

        public Panel()
        {
            _Items = new List<UIElement>();
        }

        public virtual void Add(UIElement element)
        {
            element.ParentElement = this;

            _Items.Add(element);
            MeasureSize(element);
        }

        protected abstract void MeasureSize(UIElement element);

        /// <summary>
        /// Not implemented.
        /// </summary>
        protected override void MeasureSize() 
        {
            throw new NotImplementedException();
        }
    }
}
