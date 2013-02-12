using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides data for a Sprite.
	/// This class is mostly to work around issues with shared resource collections and other limitations within the Content Pipeline.
	/// </summary>
	public class SpriteData {
		/// <summary>
		/// Gets or sets the texture used for this Sprite.
		/// </summary>
		[ContentSerializer]
		public Texture2D Texture { get; set; }

		/// <summary>
		/// Provides the source rectangles for each frame.
		/// </summary>
		[ContentSerializer]
		public List<SpriteFrameData> Frames { get; set; }

		/// <summary>
		/// Indicates how long each frame, indexed by name, lasts for an animation.
		/// </summary>
		[ContentSerializerIgnore]
		public List<SpriteAnimationData> Animations { get; set; }
	}
}
