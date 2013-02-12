using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides a single frame for a Sprite animation.
	/// </summary>
	public class SpriteFrame {

		private string _Name;
		private Rectangle _Source;
		private Sprite _Sprite;

		/// <summary>
		/// Gets the name of this particular frame.
		/// </summary>
		public string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Gets the source rectangle for this frame.
		/// That is, the location within Texture that this frame is located.
		/// </summary>
		public Rectangle Source {
			get { return _Source; }
		}

		/// <summary>
		/// Gets the sprite that owns this frame.
		/// </summary>
		public Sprite Sprite {
			get { return _Sprite; }
		}

		public SpriteFrame(string Name, Rectangle Source, Sprite Sprite) {
			this._Name = Name;
			this._Source = Source;
			this._Sprite = Sprite;
		}
	}
}
