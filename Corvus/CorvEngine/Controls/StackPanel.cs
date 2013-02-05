using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    public class StackPanel : Panel
    {
        private Orientation _Orientation = Orientation.Vertical;

        public Orientation Orientation
        {
            get { return _Orientation; }
            set { _Orientation = value; }
        }

        /// <summary>
        /// Scales all the elements in the StackPanel. NOTE: THERES A BUG. You have to put all the elements into the stack BEFORE setting the scale value.
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

        public StackPanel()
            : base()
        {
            this.TabStop = false;
            this.Items = new List<UIElement>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (UIElement element in this.Items)
                element.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 newPosition = this.Position;
            foreach (UIElement element in this.Items)
            {
                element.Position = newPosition;
                element.Draw(spriteBatch);

                if (Orientation == Orientation.Vertical)
                    newPosition.Y += element.Size.Y;
                else if (Orientation == Orientation.Horizontal)
                    newPosition.X += element.Size.X;
            }
        }

        public void Add(UIElement element)
        {
            this.Items.Add(element);

            MeasureSize(element);
        }
        
        private void MeasureSize(UIElement element)
        {
            if (this.Size != Vector2.Zero)
            {
                Vector2 newPos = this.Size;
                if (Orientation == Orientation.Vertical)
                {
                    if (this.ScaledSize.X < element.ScaledSize.X)
                        newPos.X = element.ScaledSize.X;
                    newPos.Y = this.ScaledSize.Y + element.ScaledSize.Y;
                }
                else //if (Orientation == Orientation.Horizontal)
                {
                    if (this.ScaledSize.Y < element.ScaledSize.Y)
                        newPos.Y = element.ScaledSize.Y;
                    newPos.X = this.ScaledSize.X + element.ScaledSize.X;
                }
                this.Size = newPos;
            }
            else
                this.Size = element.Size;
        }
    }
}
