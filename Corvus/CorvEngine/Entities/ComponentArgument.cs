using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// Represents an argument that can be passed into a PropertyValueGenerator to be transformed recursively.
	/// </summary>
	public class ComponentArgument {

		// TODO: IMPORTANT: Make this be able to parse itself, so we can adjust instances within tiled by setting object properties.
		// Example: Name: PathComponent.Nodes Value: Collection(Vector2(50, 50), Vector2(100, 50))

		/// <summary>
		/// Returns a ComponentArgument that returns the results of the specified arguments transformed by the specified ValueGenerator.
		/// </summary>
		public ComponentArgument(PropertyValueGenerator ValueGenerator, IEnumerable<ComponentArgument> Arguments) {
			this._ValueGenerator = ValueGenerator;
			this._Arguments = Arguments.Select(c => (object)c).ToArray();
		}

		/// <summary>
		/// Creates a ComponentArgument that simply returns the specified argument without being transformed.
		/// </summary>
		public ComponentArgument(object Argument) {
			this._ValueGenerator = null;
			this._Arguments = new object[] { Argument };
		}

		/// <summary>
		/// Generates a value for this argument to be applied on the specified property and instance.
		/// This may return different results when called multiple times.
		/// </summary>
		public object GetValue(object Instance, PropertyInfo Property) {
			object[] Arguments = GetArguments(Instance, Property);
			object Result = ValueGenerator.GetValue(Instance, Property, Arguments);
			return Result;
		}

		private object[] GetArguments(object Instance, PropertyInfo Property) {
			if(_ValueGenerator == null)
				_ValueGenerator = PropertyValueGenerator.GetGenerator("Identity");
			if(_ValueGenerator is IdentityValueGenerator)
				return _Arguments;
			else
				return _Arguments.Select(c => ((ComponentArgument)c).GetValue(Instance, Property)).ToArray();
		}

		/// <summary>
		/// Gets the value generator to apply to this argument.
		/// To return only the value that was passed in, this would be null or an instance of IdentityValueGenerator.
		/// </summary>
		public PropertyValueGenerator ValueGenerator {
			get { return _ValueGenerator; }
		}

		/// <summary>
		/// Parses ComponentProperties from the specified input.
		/// The input must be in the format of 'Collection(Vector(2, 2), Vector(4, 4))'.
		/// Multiple arguments can be specified in the format of 'Vector(3, 5), 12, Color(White)'.
		/// </summary>
		public static IEnumerable<ComponentArgument> Parse(string Input) {
			// TODO: Should really prevent things like Property: Transformed(1, 2, 3) INVALIDINPUT, Blah
			// Which would be counted as 3 arguments.
			int LastArgumentStart = 0;
			for(int i = 0; i < Input.Length; i++) {
				char c = Input[i];
				if(c == '(') {
					// Starting a value argument. Find the last white-space, that's where the name of this argument is.
					// Parse the arguments of the remaining recursively.
					int MatchingIndex = GetMatchedBracketIndex(Input, i);
					string ChildContents = Input.Substring(i + 1, MatchingIndex - i - 1);
					int LastWhiteSpace = Math.Max(Input.LastIndexOfAny(new char[] { ' ', '\t' }, i), 0);
					string GeneratorName = Input.Substring(LastWhiteSpace, i - LastWhiteSpace).Trim();
					var Arguments = Parse(ChildContents).ToArray();
					var Generator = GetGenerator(GeneratorName);
					yield return new ComponentArgument(Generator, Arguments);
					// Skip to end of arguments and indicate remaining text is right after that.
					i = MatchingIndex;
					LastArgumentStart = i + 1;
					continue;
				} else if(c == ',' || i == Input.Length - 1) {
					// Remainder of text, if any.
					// This wasn't being transformed, so it just uses an IdentityValueGenerator.
					string RemainingText = Input.Substring(LastArgumentStart, i - LastArgumentStart + 1).Trim();
					if(i != Input.Length - 1) // This is a comma, we don't want to include that; but we do want to include the last character when it's that.
						RemainingText = RemainingText.Substring(0, RemainingText.Length - 1);
					if(!String.IsNullOrWhiteSpace(RemainingText)) {
						yield return new ComponentArgument(RemainingText);
					}
					LastArgumentStart = i + 1;
				}
			}
		}

		private static int GetMatchedBracketIndex(string Input, int Index) {
			char Initial = Input[Index];
			char Matched = BracketMatches[Initial];
			for(int i = Index + 1; i < Input.Length; i++) {
				char c = Input[i];
				if(c != Initial && c != Matched)
					continue;
				if(IsEscaped(Input, i))
					continue;
				if(c == Initial) {
					int Next = GetMatchedBracketIndex(Input, i);
					i = Next;
					continue;
				} else if(c == Matched)
					return i;
			}
			throw new FormatException("Did not find a '" + Matched + "' to match the value of '" + Initial + "'.");
		}

		private static bool IsEscaped(string Input, int Index) {
			bool IsEscaped = false;
			for(int i = Index - 1; i >= 0; i++) {
				if(Input[i] == '\\')
					IsEscaped = !IsEscaped;
				else
					break;
			}
			return IsEscaped;
		}

		private static PropertyValueGenerator GetGenerator(string Name) {
			PropertyValueGenerator Generator = PropertyValueGenerator.GetGenerator(Name);
			if(Generator == null)
				throw new KeyNotFoundException("Unable to find a PropertyValueGenerator named '" + Name + "'.");
			return Generator;
		}

		private static Dictionary<char, char> BracketMatches = new Dictionary<char, char>() {
			{ '{', '}' },
			{ '(', ')' },
			{ '[', ']' }
		};

		PropertyValueGenerator _ValueGenerator;
		private object[] _Arguments;
	}
}
