using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides data for a single SpriteFrame.
	/// See SpriteData for more details.
	/// </summary>
	public class SpriteFrameData {

		/// <summary>
		/// Gets or sets the name of this frame.
		/// </summary>
		[ContentSerializer]
		public string Name;// { get; set; }

		/// <summary>
		/// Gets or sets the source rectangle for this frame.
		/// </summary>
		[ContentSerializer]
		public Rectangle Source;// { get; set; }

		private SpriteFrameData() { }
	}
}
