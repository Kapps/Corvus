using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides animation information for a SpriteFrame.
	/// </summary>
	public class SpriteAnimationFrame {
		private SpriteFrame _Frame;
		private TimeSpan _Duration;

		/// <summary>
		/// Gets the frame that this animation applies to.
		/// </summary>
		public SpriteFrame Frame {
			get { return _Frame; }
		}

		/// <summary>
		/// Indicates how long this frame should play for.
		/// </summary>
		public TimeSpan Duration {
			get { return _Duration; }
		}

		public SpriteAnimationFrame(SpriteFrame Frame, TimeSpan Duration) {
			this._Frame = Frame;
			this._Duration = Duration;
		}
	}
}
