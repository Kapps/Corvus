using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SpritePipeline {
	/// <summary>
	/// Provides runtime information for a single SpriteAnimationData instance.
	/// </summary>
	[ContentSerializerRuntimeType("CorvEngine.Graphics.SpriteAnimationData, CorvEngine")]
	public class SpriteAnimationDataContent {
		public string Name { get; set; }
		public bool IsLooped { get; set; }
		public bool IsDefault { get; set; }
		public Dictionary<string, TimeSpan> FrameDurations { get; set; }
	}
}
