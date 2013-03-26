using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using Corvus.Interface.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Interface
{
    /// <summary>
    /// The horizontal alignment of the control.
    /// </summary>
    public enum HorizontalAlignment { Left, Center, Right, None }
    /// <summary>
    /// The vertical alignment of the control.
    /// </summary>
    public enum VerticalAlignment { Top, Center, Bottom, None }

    /// <summary>
    /// A helper class for user interface stuff.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Gets the viewport.
        /// </summary>
        public static Viewport Viewport { get { return CorvusGame.Instance.GraphicsDevice.Viewport; } }

        /// <summary>
        /// Aligns the control based on its size, horizontal, and vertical alignment.
        /// </summary>
        public static Vector2 AlignControl(Vector2 controlSize, HorizontalAlignment hAlign, VerticalAlignment vAlign)
        {
            Vector2 result = new Vector2();

            switch (hAlign)
            {
                case HorizontalAlignment.Left:
                    result.X = 0;
                    break;
                case HorizontalAlignment.Center:
                    result.X = Viewport.Width / 2;
                    result.X -= controlSize.X / 2;
                    break;
                case HorizontalAlignment.Right:
                    result.X = Viewport.Width;
                    result.X -= controlSize.X;
                    break;
                default:
                    break;
            }

            switch (vAlign)
            {
                case VerticalAlignment.Top:
                    result.Y = 0;
                    break;
                case VerticalAlignment.Center:
                    result.Y = Viewport.Height / 2;
                    result.Y -= controlSize.Y / 2;
                    break;
                case VerticalAlignment.Bottom:
                    result.Y = Viewport.Height;
                    result.Y -= controlSize.Y ;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
