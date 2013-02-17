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
using System.Text.RegularExpressions;

namespace SpritePipeline {
	/// <summary>
	/// Provides a ContentProcessor used to process Sprites for CorvEngine.
	/// </summary>
	[ContentProcessor(DisplayName = "Sprite - CorvEngine")]
	public class SpriteProcessor : ContentProcessor<string[], SpriteDataContent> {
		public override SpriteDataContent Process(string[] input, ContentProcessorContext context) {
/*#if DEBUG // Used to debug content pipeline stuff.
			if(!Debugger.IsAttached) 
				Debugger.Launch();
			Debugger.Break();
#endif*/
			SpriteDataContent spriteSheet = new SpriteDataContent();
			var Header = ReadHeader(input[0], context);
			PopulateFrames(spriteSheet, Header);
			PopulateAnimations(spriteSheet, Header, input.Skip(1).ToArray());
			spriteSheet.Texture = Header.Texture;
			return spriteSheet;
		}

		private void PopulateFrames(SpriteDataContent Sprite, SpriteHeader Header) {
			var TextureSize = GetTextureSize(Header.Texture);
			var GridSize = new Tuple<int, int>(TextureSize.Item1 / Header.Columns, TextureSize.Item2 / Header.Rows);
			List<SpriteFrameDataContent> Frames = new List<SpriteFrameDataContent>();
			for(int y = 0; y < Header.Rows; y++) {
				for(int x = 0; x < Header.Columns; x++) {
					int StartY = y * GridSize.Item2;
					int StartX = x * GridSize.Item1;
					Rectangle Source = new Rectangle(StartX, StartY, GridSize.Item1, GridSize.Item2);
					string FrameName = IndexToName(new Tuple<int, int>(x + 1, y + 1));
					SpriteFrameDataContent FrameData = new SpriteFrameDataContent() {
						Name = FrameName,
						Source = Source
					};
					Frames.Add(FrameData);
				}
			}
			Sprite.Frames = Frames;
		}

		private string IndexToName(Tuple<int, int> Index) {
			return "{" + (Index.Item2) + ", " + (Index.Item1) + "}";
		}

		private void PopulateAnimations(SpriteDataContent Sprite, SpriteHeader Header, IEnumerable<string> AnimationRows) {
			List<SpriteAnimationDataContent> Animations = new List<SpriteAnimationDataContent>();
			foreach(var AnimationRow in AnimationRows) {
				var Reader = CreateReader(AnimationRow);
				string AnimationName = Reader.Read();
				float Duration = float.Parse(Reader.Read());
				var Frames = ParseFrames(Header, Reader).ToArray(); // Array to process entirely so we do Reader.Read at right times.
				AnimationFlags Flags = AnimationFlags.None;
				while(Reader.HasMore()) {
					string NextFlag = Reader.Read();
					AnimationFlags Flag = (AnimationFlags)Enum.Parse(typeof(AnimationFlags), NextFlag);
					Flags |= Flag;
				}
				var FrameNames = Frames.Select(IndexToName);
				Dictionary<string, TimeSpan> FrameDurations = new Dictionary<string, TimeSpan>();
				foreach(var FrameName in FrameNames)
					FrameDurations.Add(FrameName, TimeSpan.FromMilliseconds(Duration));
				SpriteAnimationDataContent Animation = new SpriteAnimationDataContent() {
					FrameDurations = FrameDurations,
					IsDefault = (Flags & AnimationFlags.Default) != 0,
					IsLooped = (Flags & AnimationFlags.Looped) != 0,
					Name = AnimationName
				};
				Animations.Add(Animation);
			}
			Sprite.Animations = Animations;
		}

		private IEnumerable<Tuple<int, int>> ParseFrames(SpriteHeader Header, ColumnReader Reader) {
			// First, handle doing an entire row or column.
			string FrameData = Reader.Read();
			switch(FrameData[0]) {
				case 'R':
					int RowNum = int.Parse(FrameData.Substring(1));
					for(int i = 1; i <= Header.Columns; i++)
						yield return new Tuple<int, int>(i, RowNum);
					yield break;
				case 'C':
					int ColNum = int.Parse(FrameData.Substring(1));
					for(int i = 1; i <= Header.Rows; i++)
						yield return new Tuple<int, int>(ColNum, i);
					yield break;
			}
			// Then, allow for things like {1, 2}	{1, 4}.
			// When needed anyways. For now just throw.
			var Start = ReadIndex(FrameData);
			var End = ReadIndex(FrameData);
			int StartIndex = CoordsToIndex(Start, Header);
			int EndIndex = CoordsToIndex(End, Header);
			for(int i = StartIndex; i <= EndIndex; i++) {
				yield return IndexToCoords(i, Header);
			}
		}

		private int CoordsToIndex(Tuple<int, int> Coords, SpriteHeader Header) {
			return (Coords.Item2 - 1) * Header.Columns + (Coords.Item1 - 1);
		}

		private Tuple<int, int> IndexToCoords(int Index, SpriteHeader Header) {
			return new Tuple<int, int>(Index / Header.Columns + 1, Index % Header.Columns + 1);
		}

		private SpriteHeader ReadHeader(string Header, ContentProcessorContext Context) {
			int IndexTab = Header.IndexOf('\t');
			string FileName = Header.Substring(0, IndexTab);
			string RawSize = Header.Substring(IndexTab).Trim();
			var Size = ReadIndex(RawSize);
			var TextureReference = new ExternalReference<Texture2DContent>(FileName);
			var Texture = (Texture2DContent)Context.BuildAndLoadAsset<Texture2DContent, TextureContent>(TextureReference, "TextureProcessor");
			return new SpriteHeader() {
				Columns = Size.Item1,
				Rows = Size.Item2,
				Name = Path.GetFileNameWithoutExtension(FileName),
				Texture = Texture
			};
		}

		private Tuple<int, int> ReadIndex(string Input) {
			var Match = Regex.Match(Input, @"\{(\d+), (\d+)\}");
			if(!Match.Success)
				throw new InvalidDataException("Expected to receive a coordinate in the format of {1, 2}.");
			int X = int.Parse(Match.Groups[1].Value);
			int Y = int.Parse(Match.Groups[2].Value);
			return new Tuple<int, int>(X, Y);
		}

		private Tuple<int, int> GetTextureSize(Texture2DContent Content) {
			return new Tuple<int, int>(Content.Mipmaps[0].Width, Content.Mipmaps[0].Height);
		}

		private ColumnReader CreateReader(string Input) {
			return new ColumnReader(Input.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// Provides a wrapper around a string array that reads columns sequentially.
		/// </summary>
		private class ColumnReader {
			private int Index;
			private string[] Columns;

			public ColumnReader(string[] Columns) {
				this.Index = 0;
				this.Columns = Columns;
			}

			public string Read() {
				return Columns[Index++];
			}

			public bool HasMore() {
				return Index < Columns.Length;
			}
		}

		/// <summary>
		/// Provides information about the header of a sprite, such as the name and number of rows and columns.
		/// </summary>
		private struct SpriteHeader {
			public Texture2DContent Texture;
			public string Name;
			public int Rows;
			public int Columns;
		}

		/// <summary>
		/// Provides additional, optional, information for an animation.
		/// </summary>
		[Flags]
		private enum AnimationFlags {
			None,
			Looped,
			Default
		}
	}
}