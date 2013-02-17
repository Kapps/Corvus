﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a parser that can be used to parse EntityBlueprints and their corresponding ComponentBlueprints from a file.
	/// </summary>
	public static class BlueprintParser {
		private static List<string> ComponentNamespaces = GenerateComponentNamespaces();

		private static List<string> GenerateComponentNamespaces() {
			HashSet<string> Result = new HashSet<string>();
			foreach(var Assembly in AssemblyManager.GetAssembliesToSearch()) {
				foreach(var Type in Assembly.GetTypes()) {
					if(Type.IsSubclassOf(typeof(Component))) {
						Result.Add(Type.Namespace);
					}
				}
			}
			Result.Add(""); // Add an empty namespace in case they specifically specify.
			return Result.ToList();
		}

		/// <summary>
		/// Parses a blueprint from the specified stream, registering all EntityBlueprints contained by the Stream (and replacing any existing data for them).
		/// The stream is not closed upon completion, and may have multiple EntityBlueprints defined within it.
		/// </summary>
		public static void ParseBlueprint(string Contents) {
			LineReader Reader = new LineReader(Contents.Split(new char[] { '\r', '\n' }));
			while(GetNextScope(Reader) != LineScope.EndOfFile) {
				// Parse the Entity header, such as name and what it inherits.
				BlueprintHeader Header = ParseHeader(Reader);
				// Parse all components, including properties.
				List<ComponentBlueprint> Components = new List<ComponentBlueprint>();
				while(GetNextScope(Reader) == LineScope.Component) {
					var CurrentComponent = ParseComponentHeader(Reader);
					List<ComponentProperty> Properties = new List<ComponentProperty>();
					while(GetNextScope(Reader) == LineScope.Property) {
						var Property = ParseProperty(CurrentComponent, Reader);
						Properties.Add(Property);
					}
					Type ComponentType = ResolveType(CurrentComponent.Type);
					var ComponentBlueprint = new ComponentBlueprint(ComponentType, CurrentComponent.Name, Properties);
					Components.Add(ComponentBlueprint);
				}

				ApplyInheritance(Header, Components);
				var Entity = EntityBlueprint.CreateBlueprint(Header.Name, Components);
			}
		}

		private static void ApplyInheritance(BlueprintHeader Header, List<ComponentBlueprint> Components) {
			// Go through each inheritance (in order) and add missing components / properties.
			foreach(var Inheritance in Header.Inherits) {
				EntityBlueprint InheritedBlueprint = EntityBlueprint.GetBlueprint(Inheritance);
				if(InheritedBlueprint == null)
					throw new KeyNotFoundException("Unable to find inherited blueprint '" + Inheritance + "'. Please note inherited blueprints must appear above this blueprint in the blueprint specification.");
				foreach(var Component in InheritedBlueprint.Components) {
					var ExistingComponent = Components.FirstOrDefault(c => c.Name.Equals(Component.Name, StringComparison.InvariantCultureIgnoreCase));
					if(ExistingComponent == null) {
						// Just copy over the complete component.
						Components.Add(Component);
					} else {
						// Otherwise, we already that component, so copy over properties we're missing.
						foreach(var Property in Component.Properties) {
							var ExistingProperty = ExistingComponent.Properties.FirstOrDefault(c => c.PropertyName.Equals(Property.PropertyName, StringComparison.InvariantCultureIgnoreCase));
							if(ExistingProperty == null) {
								// Don't have the property, so add it. Otherwise, keep our own value.
								ExistingComponent.Properties.Add(Property);
							}
						}
					}
				}
			}
		}

		private static LineScope GetNextScope(LineReader Reader) {
			string Line = Reader.PeekLine();
			if(Line == null)
				return LineScope.EndOfFile;
			int Result = Line.TakeWhile(c => c == '\t').Count();
			if(Result > (int)LineScope.Property)
				throw new InvalidDataException("Found too many scopes for line '" + Line + "'.");
			return (LineScope)Result;
		}

		private static Type ResolveType(string Name) {
			foreach(var Assembly in AssemblyManager.GetAssembliesToSearch()) {
				foreach(var Namespace in ComponentNamespaces) {
					string QualifiedName = (String.IsNullOrWhiteSpace(Namespace) ? Name : (Namespace + "." + Name));
					Type type = Assembly.GetType(QualifiedName, false, true);
					if(type != null)
						return type;
				}
			}
			throw new TypeLoadException("Unable to find the type referenced by '" + Name + "'.", null);
		}

		private static ComponentHeader ParseComponentHeader(LineReader Reader) {
			string Line = Reader.ReadLine().Trim();
			var IndexFirstSpace = Line.IndexOfAny(new char[] { ' ', '\t' });
			string Type, Name;
			if(IndexFirstSpace == -1) {
				Type = Line.Trim();
				Name = Type;
			} else {
				Type = Line.Substring(0, IndexFirstSpace).Trim();
				Line = Line.Substring(IndexFirstSpace).Trim();
				if(Line[0] != '(')
					throw new InvalidDataException("Expected first character past Component name to be ( for renaming.");
				int NameEnd = Line.IndexOf(')');
				if(NameEnd == -1)
					throw new InvalidDataException("Expected closing ) to match rename start for Component.");
				Name = Line.Substring(1, NameEnd - 1).Trim();
			}
			return new ComponentHeader() {
				Name = Name,
				Type = Type
			};
		}

		private static ComponentProperty ParseProperty(ComponentHeader CurrentComponent, LineReader Reader) {
			string Line = Reader.ReadLine().Trim();
			var Regex = new Regex(@"^(.+)\w*:\s*(\w+)?(\(.+\))?");
			var Match = Regex.Match(Line);
			var Groups = Match.Groups;
			string Name = Match.Groups[1].Value;
			string GeneratorName = "Identity";
			object[] Arguments = new object[0];
			if(Groups.Count != 4)
				throw new InvalidDataException("Expected regex to return 4 groups. This is either a bug in the regex, or invalid input for a property specifier.");
			if(Groups[3].Success) {
				// We specified a generator:
				GeneratorName = Groups[2].Value;
				string ArgumentText = Groups[3].Value;
				ArgumentText = ArgumentText.Substring(1, ArgumentText.Length - 2);
				Arguments = ArgumentText.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(c=>(object)c).ToArray();
			}
			PropertyValueGenerator Generator = PropertyValueGenerator.GetGenerator(GeneratorName);
			if(Generator == null)
				throw new KeyNotFoundException("Unable to find a PropertyValueGenerator named '" + GeneratorName + "'.");
			return new ComponentProperty(CurrentComponent.Name, Name, Generator, Arguments);
		}

		private static BlueprintHeader ParseHeader(LineReader Reader) {
			var Line = Reader.ReadLine();
			var IndexFirstSpace = Line.IndexOfAny(new char[] { ' ', '\t' });
			string Name = IndexFirstSpace == -1 ? Line.Trim() : Line.Substring(0, IndexFirstSpace).Trim();
			string[] Inherits = new string[0];
			Line = IndexFirstSpace == -1 ? "" : Line.Substring(IndexFirstSpace).Trim();
			if(Line.Trim().Length > 1 && Line[0] == ':') {
				Inherits = Line.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();
			}
			return new BlueprintHeader() {
				Name = Name,
				Inherits = Inherits
			};
		}

		private struct BlueprintHeader {
			public string Name;
			public string[] Inherits;
		}

		private struct ComponentHeader {
			public string Name;
			public string Type;
		}

		private class LineReader {
			private string[] Lines;
			private int Index;

			public string ReadLine() {
				while(true) {
					if(Index >= Lines.Length)
						return null;
					string Result = Lines[Index];
					int IndexComment = Result.IndexOf('#');
					if(IndexComment != -1)
						Result = Result.Substring(0, IndexComment);
					Index++;
					if(String.IsNullOrWhiteSpace(Result))
						continue;
					return Result;
				}
			}

			public string PeekLine() {
				int Old = Index;
				try {
					return ReadLine();
				} finally {
					Index = Old;
				}
			}

			public bool HasLinesRemaining() {
				return PeekLine() != null;
			}

			public LineReader(string[] Lines) {
				this.Lines = Lines;
			}
		}

		private enum LineScope {
			Entity = 0,
			Component = 1,
			Property = 2,
			EndOfFile = 3
		}
	}
}
