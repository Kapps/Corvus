using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SpritePipeline {
	/// <summary>
	/// Provides runtime information for a single SpriteFrameData instance.
	/// </summary>
	[ContentSerializerRuntimeType("CorvEngine.Graphics.SpriteFrameData, CorvEngine")]
	public class SpriteFrameDataContent {
		public string Name { get; set; }
		public Rectangle Source { get; set; }
	}
}
