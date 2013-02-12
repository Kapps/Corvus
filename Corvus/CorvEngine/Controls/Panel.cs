using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    /// <summary>
    /// A base class for elements that contain elements.
    /// </summary>
    public abstract class Panel : UIElement
    {
        private List<UIElement> _Items;

        /// <summary>
        /// The list of items in this panel.
        /// </summary>
        public virtual List<UIElement> Items 
        { 
            get { return _Items; }
            protected set { _Items = value; }
        }

        /// <summary>
        /// Determines if this element has focus.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of Panel.
        /// </summary>
        public Panel()
        {
            _Items = new List<UIElement>();
        }

        /// <summary>
        /// Adds the element to the panel.
        /// </summary>
        public virtual void Add(UIElement element)
        {
            element.ParentElement = this;

            _Items.Add(element);
            MeasureSize(element);
        }

        /// <summary>
        /// Measures the size of the Panel as elements are being added.
        /// </summary>
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
