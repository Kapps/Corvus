using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using CorvEngine.Components;
using CorvEngine.Components.Blueprints;
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
        /// Returns an array of all the properties defined within this level.
        /// </summary>
        public LevelProperty[] Properties { get; set; }

		/// <summary>
		/// Loads the data for a level using the Tiled Map Xml format.
		/// </summary>
		public static LevelData LoadTmx(string FilePath) {
			// https://github.com/bjorn/tiled/wiki/TMX-Map-Format contains a description of the format.
			XmlDocument Doc = new XmlDocument();
			Doc.Load(FilePath);
			if(Doc.GetElementsByTagName("map").Count != 1)
				throw new FormatException("Expected a single map to be defined in a .tmx file.");
			var MapElement = Doc.GetElementsByTagName("map").Item(0);
			var MapDetails = new MapDetails(MapElement);
			List<TextureDetails> Textures = ParseTilesets(MapDetails);
			List<Layer> Layers = ParseLayers(MapDetails, Textures);
			List<Entity> Entities = ParseEntities(MapDetails);
            List<LevelProperty> Properties = ParseProperties(MapDetails);

			LevelData Result = new LevelData() {
				DynamicObjects = Entities.ToArray(),
				Layers = Layers.ToArray(),
				MapSize = new Vector2(MapDetails.MapWidth, MapDetails.MapHeight),
				TileSize = new Vector2(MapDetails.MapTileWidth, MapDetails.MapTileHeight),
                Properties = Properties.ToArray()
			};
			return Result;
		}

        private static List<LevelProperty> ParseProperties(MapDetails Map)
        {
            List<LevelProperty> Result = new List<LevelProperty>();
            foreach (XmlNode propertiesNode in Map.MapElement.SelectNodes("properties"))
            {
                foreach (XmlNode propertyNode in propertiesNode.SelectNodes("property"))
                {
                    string name = propertyNode.Attributes["name"].Value;
                    string value = propertyNode.Attributes["value"].Value;
                    Result.Add(new LevelProperty(name, value));
                }
            }

            return Result;
        }

		private static List<Entity> ParseEntities(MapDetails Map) {
			List<Entity> Entities = new List<Entity>();
			List<PathDetails> Paths = new List<PathDetails>();
			foreach(XmlNode ObjectGroupNode in Map.MapElement.SelectNodes("objectgroup")) {
				foreach(XmlNode ObjectNode in ObjectGroupNode.SelectNodes("object")) {
					ObjectType Type = ReadObjectType(ObjectNode);
					if(Type == ObjectType.Path) {
						var Path = ParsePath(ObjectNode);
						Paths.Add(Path);
					} else if(Type == ObjectType.Entity) {
						var Entity = ParseEntity(ObjectNode);
						Entities.Add(Entity);
					} else
						throw new ArgumentException("Unknown object type '" + Type + "'.");
				}
			}

			foreach(var Path in Paths) {
				var Entity = Entities.Where(c => c.Name.Equals(Path.EntityName, StringComparison.InvariantCultureIgnoreCase));
                foreach (var e in Entity)
                {
                    var PathComponent = e.GetComponent<PathComponent>();
                    if (PathComponent == null)
                    {
                        PathComponent = new PathComponent();
                        e.Components.Add(PathComponent);
                    }
                    if (PathComponent.Nodes != null)
                        PathComponent.Nodes.Clear();
                    foreach (var Node in Path.Nodes)
                        PathComponent.AddNode(Node);
                }
			}
			return Entities;
		}

		private static PathDetails ParsePath(XmlNode ObjectNode) {
			float X = float.Parse(ObjectNode.Attributes["x"].Value);
			float Y = float.Parse(ObjectNode.Attributes["y"].Value);
			string EntityName = ObjectNode.Attributes["name"].Value.Trim();
			List<Vector2> Nodes = new List<Vector2>();
			foreach(var PointText in ObjectNode.SelectSingleNode("polyline").Attributes["points"].Value.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)) {
				int IndexComma = PointText.IndexOf(',');
				float NodeX = float.Parse(PointText.Substring(0, IndexComma).Trim());
				float NodeY = float.Parse(PointText.Substring(IndexComma + 1).Trim());
				Nodes.Add(new Vector2(NodeX + X, NodeY + Y));
			}
			return new PathDetails() {
				EntityName = EntityName,
				Nodes = Nodes
			};
		}

		private static Entity ParseEntity(XmlNode ObjectNode) {
			float Width = float.Parse(ObjectNode.Attributes["width"].Value);
			float Height = float.Parse(ObjectNode.Attributes["height"].Value);
			float X = float.Parse(ObjectNode.Attributes["x"].Value);
			float Y = float.Parse(ObjectNode.Attributes["y"].Value);
			string BlueprintName = ObjectNode.Attributes["type"].Value.Trim();
			string EntityName = ObjectNode.Attributes["name"] == null ? null : ObjectNode.Attributes["name"].Value.Trim();
			EntityBlueprint Blueprint = EntityBlueprint.GetBlueprint(BlueprintName);
			Entity Entity = Blueprint.CreateEntity();
			if(!String.IsNullOrWhiteSpace(EntityName))
				Entity.Name = EntityName;
			Entity.Position = new Vector2(X, Y);
			Entity.Size = new Vector2(Width, Height);

			foreach(XmlNode PropertiesNode in ObjectNode.SelectNodes("properties")) {
				foreach(XmlNode PropertyNode in PropertiesNode.SelectNodes("property")) {
					string Name = PropertyNode.Attributes["name"].Value.Trim();
					// So, this is quite a hack.
					// Tiled doesn't allow us to re-order properties; it's all alphabetical.
					// So we just support sticking a # in front of the property to make it go to the top of the list, and then ignore that #.
					while(Name.FirstOrDefault() == '#')
						Name = Name.Substring(1);
					string Value = PropertyNode.Attributes["value"].Value.Trim();
					string[] NamePropertySplit = Name.Split('.');
					if(NamePropertySplit.Length != 2)
						throw new FormatException("Expected object property name to be in the format of 'PathComponent.Nodes'.");
					string ComponentName = NamePropertySplit[0].Trim();
					string PropertyName = NamePropertySplit[1].Trim();
					ComponentArgument Argument = ComponentArgument.Parse(Value).Single();
					ComponentProperty ParsedProperty = new ComponentProperty(ComponentName, PropertyName, Argument);
					var Component = Entity.Components[ComponentName];
					ParsedProperty.ApplyValue(Component);
				}
			}
			return Entity;
		}

		private static ObjectType ReadObjectType(XmlNode ObjectNode) {
			string TypeAttrib = ObjectNode.Attributes["type"].Value;
			if(TypeAttrib.Equals("Path", StringComparison.InvariantCultureIgnoreCase))
				return ObjectType.Path;
			return ObjectType.Entity;
		}

		private static List<Layer> ParseLayers(MapDetails Map, List<TextureDetails> Textures) {
			var Result = new List<Layer>();
			foreach(XmlNode LayerNode in Map.MapElement.SelectNodes("layer")) {
				var DataNode = LayerNode.SelectNodes("data").Item(0);
				string CompressionFormat = DataNode.Attributes["compression"].Value;
				string EncodingFormat = DataNode.Attributes["encoding"].Value;
				if(!CompressionFormat.Equals("gzip", StringComparison.InvariantCultureIgnoreCase) || !EncodingFormat.Equals("base64", StringComparison.InvariantCultureIgnoreCase))
					throw new FormatException("Currently the Tmx loader can only handled base-64 zlib tiles.");
				string Base64Data = DataNode.InnerXml.Trim();
				byte[] CompressedData = Convert.FromBase64String(Base64Data);
				byte[] UncompressedData = new byte[1024]; // NOTE: This must be a multiple of 4.
				Tile[,] Tiles = new Tile[Map.MapNumTilesWide, Map.MapNumTilesHigh];
				int MapIndex = 0;
				using(var GZipStream = new GZipStream(new MemoryStream(CompressedData), CompressionMode.Decompress, false)) {
					while(true) {
						int BytesRead = GZipStream.Read(UncompressedData, 0, UncompressedData.Length);
						if(BytesRead == 0)
							break;
						using(BinaryReader Reader = new BinaryReader(new MemoryStream(UncompressedData))) {
							for(int i = 0; i < BytesRead; i += 4) {
								int GID = Reader.ReadInt32();
								int MapX = MapIndex % Map.MapNumTilesWide;
								int MapY = MapIndex / Map.MapNumTilesWide;
								MapIndex++;
								if(GID == 0)
									continue;
								var Texture = Textures.Last(c => c.StartGID <= GID);
								int TextureX = (GID - Texture.StartGID) % Texture.NumTilesWide;
								int TextureY = (GID - Texture.StartGID) / Texture.NumTilesWide;
								Rectangle SourceRect = new Rectangle(TextureX * Texture.TileWidth, TextureY * Texture.TileHeight, Texture.TileWidth, Texture.TileHeight);
								Rectangle Location = new Rectangle(MapX * Map.MapTileWidth, MapY * Map.MapTileHeight, Map.MapTileWidth, Map.MapTileHeight);
								Tile Tile = new Tile(Texture.Texture, SourceRect, Location, new Vector2(MapX, MapY));
								Tiles[MapX, MapY] = Tile;
							}
						}
					}
				}

				bool IsSolid = true;
				foreach(XmlNode PropertiesNode in LayerNode.SelectNodes("properties")) {
					foreach(XmlNode Property in PropertiesNode.SelectNodes("property")) {
						string Name = Property.Attributes["name"].Value;
						string Value = Property.Attributes["value"].Value;
						if(Name.Equals("Solid", StringComparison.InvariantCultureIgnoreCase)) {
							IsSolid = bool.Parse(Value);
						}
					}
				}
				Layer Layer = new Layer(new Vector2(Map.MapTileWidth, Map.MapTileHeight), Tiles);
				Layer.IsSolid = IsSolid;
				Result.Add(Layer);
			}
			return Result;
		}

		private static List<TextureDetails> ParseTilesets(MapDetails Details) {
			var Textures = new List<TextureDetails>();
			foreach(XmlNode TilesetNode in Details.MapElement.SelectNodes("tileset")) {
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
					NumTilesHigh = Texture.Height / TileHeight,
					NumTilesWide = Texture.Width / TileWidth
				});
			}
			Textures = Textures.OrderBy(c => c.StartGID).ToList();
			return Textures;
		}

		private struct MapDetails {
			public int MapNumTilesWide;
			public int MapNumTilesHigh;
			public int MapTileWidth;
			public int MapTileHeight;
			public int MapWidth;
			public int MapHeight;
			public XmlNode MapElement;

			public MapDetails(XmlNode MapElement) {
				this.MapNumTilesWide = int.Parse(MapElement.Attributes["width"].Value);
				this.MapNumTilesHigh = int.Parse(MapElement.Attributes["height"].Value);
				this.MapTileWidth = int.Parse(MapElement.Attributes["tilewidth"].Value);
				this.MapTileHeight = int.Parse(MapElement.Attributes["tileheight"].Value);
				this.MapWidth = MapTileWidth * MapNumTilesWide;
				this.MapHeight = MapTileHeight * MapNumTilesHigh;
				this.MapElement = MapElement;
			}
		}

		private struct TextureDetails {
			public Texture2D Texture;
			public int StartGID;
			public int TileWidth;
			public int TileHeight;
			public int NumTilesWide;
			public int NumTilesHigh;
		}

		private struct PathDetails {
			public string EntityName;
			public List<Vector2> Nodes;
		}

		private enum ObjectType {
			Entity,
			Path
		}
	}
}