using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace SpritePipeline {
	/// <summary>
	/// Provides content information for Sprite data.
	/// </summary>
	[ContentSerializerRuntimeType("CorvEngine.Graphics.SpriteData, CorvEngine")]
	public class SpriteDataContent {
		public Texture2DContent Texture { get; set; }
		public List<SpriteFrameDataContent> Frames { get; set; }
		public List<SpriteAnimationDataContent> Animations { get; set; }
	}
}
