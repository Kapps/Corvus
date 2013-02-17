using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Controls
{
    /// <summary>
    /// A class to specify the spacing between elements.
    /// </summary>
    public class Thickness
    {
        private float _Left = 0;
        private float _Top = 0;
        private float _Right = 0;
        private float _Bottom = 0;

        /// <summary>
        /// The spacing with respect to the left.
        /// </summary>
        public float Left
        {
            get { return _Left; }
            set { _Left = value; }
        }

        /// <summary>
        /// The spacing with respect to the top.
        /// </summary>
        public float Top
        {
            get { return _Top; }
            set { _Top = value; }
        }

        /// <summary>
        /// The spacing with respect to the right.
        /// </summary>
        public float Right
        {
            get { return _Right; }
            set { _Right = value; }
        }

        /// <summary>
        /// The spacing with respect to the bottom.
        /// </summary>
        public float Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }

        /// <summary>
        /// Gets to total length of both the left and right spacings.
        /// </summary>
        public float GetHorizontalLength { get { return Left + Right; } }

        /// <summary>
        /// Gets to total length of both the top and bottom spacings.
        /// </summary>
        public float GetVerticalLength { get { return Top + Bottom; } }

        /// <summary>
        /// Creates a new instance of Thickness.
        /// </summary>
        public Thickness() { }

        /// <summary>
        /// Creates a new instance of Thickness with a uniform spacing.
        /// </summary>
        public Thickness(float margin)
        {
            this._Left = margin;
            this._Top = margin;
            this._Right = margin;
            this._Bottom = margin;
        }

        /// <summary>
        /// Creates a new instance of thickness with varying spacing.
        /// </summary>
        public Thickness(float left, float top, float right, float bot)
        {
            this._Left = left;
            this._Top = top;
            this._Right = right;
            this._Bottom = bot;
        }

    }
}
