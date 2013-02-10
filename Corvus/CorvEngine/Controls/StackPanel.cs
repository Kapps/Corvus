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

        public StackPanel()
            : base()
        {
            this.TabStop = false;
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
                if(element.IsVisible)
                    element.Draw(spriteBatch);

                if (Orientation == Orientation.Vertical)
                    newPosition.Y += element.Size.Y;
                else if (Orientation == Orientation.Horizontal)
                    newPosition.X += element.Size.X;
            }
        }

        protected override void MeasureSize(UIElement element)
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
