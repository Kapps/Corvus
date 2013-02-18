using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using CorvEngine.Entities;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides data about a level, such as the tiles and entities within it.
	/// </summary>
	public class LevelData {
		/// <summary>
		/// Indicates the size, in pixels, of each tile within the level.
		/// </summary>
		public Vector2 TileSize { get; set; }
		/// <summary>
		/// Indicates the size, in pixels, of the map itself.
		/// </summary>
		public Vector2 MapSize { get; set; }
		/// <summary>
		/// Returns an array of the layers contained within this level.
		/// </summary>
		public Layer[] Layers { get; set; }
		/// <summary>
		/// Returns an array of all the dynamic objects defined within this level.
		/// </summary>
		public Entity[] DynamicObjects { get; set; }

		/// <summary>
		/// Loads the data for a level using the Tiled Map Xml format.
		/// </summary>
		public static LevelData LoadTmx(string FilePath) {
			// This is really just a terrible method that should be refactored a lot at some point, at least to split layer / tileset / object loading.
			XmlDocument Doc = new XmlDocument();
			Doc.Load(FilePath);
			if(Doc.GetElementsByTagName("map").Count != 1)
				throw new FormatException("Expected a single map to be defined in a .tmx file.");
			List<TextureDetails> Textures = new List<TextureDetails>();
			List<Layer> Layers = new List<Layer>();
			List<Entity> Entities = new List<Entity>();
			var MapElement = Doc.GetElementsByTagName("map").Item(0);
			int MapNumTilesWide = int.Parse(MapElement.Attributes["width"].Value);
			int MapNumTilesHigh = int.Parse(MapElement.Attributes["height"].Value);
			int MapTileWidth = int.Parse(MapElement.Attributes["tilewidth"].Value);
			int MapTileHeight = int.Parse(MapElement.Attributes["tileheight"].Value);
			int MapWidth = MapTileWidth * MapNumTilesWide;
			int MapHeight = MapTileHeight * MapNumTilesHigh;
			foreach(XmlNode TilesetNode in MapElement.SelectNodes("tileset")) {
				if(TilesetNode.ChildNodes.Count != 1)
					throw new FormatException("Expected a single 'image' child for the children of map.");
				var ImageNode = TilesetNode.ChildNodes[0];
				string ImageSource = ImageNode.Attributes["source"].Value;
				string TextureName = Path.GetFileNameWithoutExtension(ImageSource);
				var Texture = CorvBase.Instance.GlobalContent.Load<Texture2D>("Tiles/" + TextureName);
				int StartGID = int.Parse(TilesetNode.Attributes["firstgid"].Value);
				int TileWidth = int.Parse(TilesetNode.Attributes["tilewidth"].Value);
				int TileHeight = int.Parse(TilesetNode.Attributes["tileheight"].Value);
				Textures.Add(new TextureDetails() {
					Texture = Texture,
					StartGID = StartGID,
					TileHeight = TileHeight,
					TileWidth = TileWidth,
					NumTilesHigh = MapHeight / TileHeight,
					NumTilesWide = MapWidth / TileWidth
				});
			}
			Textures = Textures.OrderBy(c => c.StartGID).ToList();
			foreach(XmlNode LayerNode in MapElement.SelectNodes("layer")) {
				if(LayerNode.ChildNodes.Count != 1)
					throw new FormatException("Expected a single 'data' child for the children of layer.");
				var DataNode = LayerNode.ChildNodes[0];
				string CompressionFormat = DataNode.Attributes["compression"].Value;
				string EncodingFormat = DataNode.Attributes["encoding"].Value;
				if(!CompressionFormat.Equals("gzip", StringComparison.InvariantCultureIgnoreCase) || !EncodingFormat.Equals("base64", StringComparison.InvariantCultureIgnoreCase))
					throw new FormatException("Currently the Tmx loader can only handled base-64 zlib tiles.");
				string Base64Data = DataNode.InnerXml.Trim();
				byte[] CompressedData = Convert.FromBase64String(Base64Data);
				byte[] UncompressedData = new byte[1024]; // NOTE: This must be a multiple of 4.
				Tile[,] Tiles = new Tile[MapNumTilesWide, MapNumTilesHigh];
				int MapIndex = 0;
				using(var GZipStream = new GZipStream(new MemoryStream(CompressedData), CompressionMode.Decompress, false)) {
					while(true) {
						int BytesRead = GZipStream.Read(UncompressedData, 0, UncompressedData.Length);
						if(BytesRead == 0)
							break;
						using(BinaryReader Reader = new BinaryReader(new MemoryStream(UncompressedData))) {
							while(Reader.BaseStream.Position != Reader.BaseStream.Length) {
								MapIndex++;
								int GID = Reader.ReadInt32();
								if(GID == 0)
									continue;
								var Texture = Textures.Last(c => c.StartGID <= GID);
								int TextureX = (GID - Texture.StartGID) % Texture.NumTilesWide;
								int TextureY = (GID - Texture.StartGID) / Texture.NumTilesWide;
								int MapX = MapIndex % MapNumTilesWide;
								int MapY = MapIndex / MapNumTilesHigh;
								Rectangle SourceRect = new Rectangle(TextureX * Texture.TileWidth, TextureY * Texture.TileHeight, Texture.TileWidth, Texture.TileHeight);
								Rectangle Location = new Rectangle(MapX * MapTileWidth, MapY * MapTileHeight, MapTileWidth, MapTileHeight);
								Tile Tile = new Tile(Texture.Texture, SourceRect, Location);
								Tiles[MapX, MapY] = Tile;
							}
						}
					}
				}
				
				Layer Layer = new Layer(Tiles);
				Layers.Add(Layer);
			}

			foreach(XmlNode ObjectGroupNode in MapElement.SelectNodes("objectgroup")) {
				foreach(XmlNode ObjectNode in ObjectGroupNode.SelectNodes("object")) {
					int Width = int.Parse(ObjectNode.Attributes["width"].Value);
					int Height = int.Parse(ObjectNode.Attributes["height"].Value);
					int X = int.Parse(ObjectNode.Attributes["x"].Value);
					int Y = int.Parse(ObjectNode.Attributes["y"].Value);
					string BlueprintName = ObjectNode.Attributes["type"].Value;
					EntityBlueprint Blueprint = EntityBlueprint.GetBlueprint(BlueprintName);
					Entity Entity = Blueprint.CreateEntity();
					Entity.Position = new Vector2(X, Y);
					Entity.Size = new Vector2(Width, Height);
					Entities.Add(Entity);
				}
			}

			LevelData Result = new LevelData() {
				DynamicObjects = Entities.ToArray(),
				Layers = Layers.ToArray(),
				MapSize = new Vector2(MapWidth, MapHeight),
				TileSize = new Vector2(MapTileWidth, MapTileHeight)
			};
			return Result;
		}

		private struct TextureDetails {
			public Texture2D Texture;
			public int StartGID;
			public int TileWidth;
			public int TileHeight;
			public int NumTilesWide;
			public int NumTilesHigh;
		}
	}
}
