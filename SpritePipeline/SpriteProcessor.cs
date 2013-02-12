#region Credits
/**
 * 
 * This class heavily based off of the SpriteSheetProcessor class from Microsoft's SpriteSheet sample.
 * http://xbox.create.msdn.com/en-US/education/catalog/sample/sprite_sheet
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;
using System.Diagnostics;

namespace SpritePipeline {
	/// <summary>
	/// This class will be instantiated by the XNA Framework Content Pipeline
	/// to apply custom processing to content data, converting an object of
	/// type TInput to TOutput. The input and output types may be the same if
	/// the processor wishes to alter data without changing its type.
	///
	/// This should be part of a Content Pipeline Extension Library project.
	/// </summary>
	[ContentProcessor(DisplayName = "Sprite Processor - CorvEngine")]
	public class SpriteProcessor : ContentProcessor<string[], SpriteDataContent> {
		public override SpriteDataContent Process(string[] input, ContentProcessorContext context) {
#if DEBUG
			//Debugger.Break();
#endif
			SpriteDataContent spriteSheet = new SpriteDataContent();
			List<BitmapContent> sourceSprites = new List<BitmapContent>();
			Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
			// Loop over each input sprite filename.
			foreach(string inputFilename in input) {
				// Store the name of this sprite.
				string spriteName = Path.GetFileNameWithoutExtension(inputFilename);
				NameToIndex.Add(spriteName, sourceSprites.Count);

				// Load the sprite texture into memory.
				ExternalReference<TextureContent> textureReference =
								new ExternalReference<TextureContent>(inputFilename);

				TextureContent texture =
					context.BuildAndLoadAsset<TextureContent,
											  TextureContent>(textureReference, "TextureProcessor");

				sourceSprites.Add(texture.Faces[0][0]);
			}

			List<Rectangle> SpriteRects = new List<Rectangle>();
			// Pack all the sprites into a single large texture.
			BitmapContent packedSprites = SpritePacker.PackSprites(sourceSprites, SpriteRects, context);
										
			var Texture = new Texture2DContent();
			Texture.Mipmaps.Add(packedSprites);
			spriteSheet.Texture = Texture;

			spriteSheet.Frames = new List<SpriteFrameDataContent>();
			foreach(var KVP in NameToIndex) {
				var Rect = SpriteRects[KVP.Value];
				var Name = KVP.Key;
				SpriteFrameDataContent Frame = new SpriteFrameDataContent() {
					Name = Name,
					Source = Rect
				};
				spriteSheet.Frames.Add(Frame);
			}

			return spriteSheet;
		}
	}
}