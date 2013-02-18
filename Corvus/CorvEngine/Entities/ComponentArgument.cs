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
		/// Generates a value for this argument.
		/// This may return different results when called multiple times.
		/// </summary>
		public object GetValue() {
			object[] Arguments = GetArguments();
			PropertyInfo Property = typeof(ComponentArgument).GetProperty("GetValue", BindingFlags.Public | BindingFlags.Instance);
			object Instance = this;

			object Result = ValueGenerator.GetValue(Instance, Property, Arguments);
			return Result;
		}

		private object[] GetArguments() {
			if(_ValueGenerator == null)
				_ValueGenerator = PropertyValueGenerator.GetGenerator("Identity");
			if(_ValueGenerator is IdentityValueGenerator)
				return _Arguments;
			else
				return _Arguments.Select(c => ((ComponentArgument)c).GetValue()).ToArray();
		}

		/// <summary>
		/// Gets the value generator to apply to this argument.
		/// To return only the value that was passed in, this would be null or an instance of IdentityValueGenerator.
		/// </summary>
		public PropertyValueGenerator ValueGenerator {
			get { return _ValueGenerator; }
		}

		PropertyValueGenerator _ValueGenerator;
		private object[] _Arguments;
	}
}
