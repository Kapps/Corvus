using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Controls
{
    /// <summary>
    /// A class to display a multiline block of text.
    /// </summary>
    public class TextBlock : UIElement
    {
        private string _Text = "";
        private int _Length = 50;
        private List<string> _Block = new List<string>();

        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set 
            { 
                _Text = value;

                SetBlock();
                MeasureSize();
            }
        }

        /// <summary>
        /// The number of characters before starting a new line. Default at 50.
        /// </summary>
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        /// <summary>
        /// Creates a new TextBlock.
        /// </summary>
        public TextBlock()
            :base()
        {
            this.TabStop = false;
            this.Size = new Vector2();
        }

        /// <summary>
        /// Creates a new Textblock with the specified length.
        /// </summary>
        public TextBlock(int length)
            : this()
        {
            this._Length = length;
        }

        /// <summary>
        /// Updates the textblock.
        /// </summary>
        public override void Update(GameTime gameTime)
        { }

        /// <summary>
        /// Draws the text.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = this.Position;
            foreach (string s in _Block)
            {
                spriteBatch.DrawString(this.SpriteFont, s, pos, this.Foreground, 0f, new Vector2(), this.Scale, SpriteEffects.None, 0f);
                pos.Y += this.Size.Y / _Block.Count(); //so i don't have to call Spritefont.Measurestring() over and over.
            }
        }

        /// <summary>
        /// Measures the size.
        /// </summary>
        protected override void MeasureSize()
        {
            Vector2 pos = new Vector2();
            foreach (string s in _Block)
            {
                Vector2 stringSize = this.SpriteFont.MeasureString(s);
                if (pos.X < stringSize.X)
                    pos.X = stringSize.X;
                pos.Y += stringSize.Y;
            }
            this.Size = pos;
        }

        /// <summary>
        /// Creates the text block from the text entered.
        /// </summary>
        private void SetBlock()
        {
            _Block.Clear();

            string[] content = _Text.Split(' ');

            string line = "";
            foreach (string s in content)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                string curLine = s;
                curLine = curLine.Replace("\r", "");
                curLine = curLine.Replace("\n", "");
                curLine += " ";

                if ((line + curLine).Count() < _Length)
                    line += curLine;
                else
                {
                    _Block.Add(line);
                    line = curLine;
                }
            }

            if (!string.IsNullOrEmpty(line))
                _Block.Add(line);
        }

    }
}
