using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides animation data for a Sprite.
	/// See SpriteData for more details.
	/// </summary>
	public class SpriteAnimationData {
		/// <summary>
		/// Gets the name of this animation.
		/// </summary>
		[ContentSerializer]
		public string Name { get; set; }

		/// <summary>
		/// Indicates whether this animation should be played in a loop.
		/// </summary>
		[ContentSerializer]
		public bool IsLooped { get; set; }

		/// <summary>
		/// Indicates whether this animation should be considered a default animation.
		/// </summary>
		[ContentSerializer]
		public bool IsDefault { get; set; }

		/// <summary>
		/// A dictionary of the frames and the duration they should last.
		/// </summary>
		[ContentSerializer]
		public Dictionary<string, TimeSpan> FrameDurations { get; set; }

		// For the serializer.
		private SpriteAnimationData() { }
	}
}
