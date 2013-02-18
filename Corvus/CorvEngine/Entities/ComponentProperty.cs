using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a class that adjusts a Property on a Component.
	/// </summary>
	public class ComponentProperty {

		/// <summary>
		/// Gets the name of the Component that this ComponentProperty applies to.
		/// </summary>
		public string ComponentName {
			get { return _ComponentName; }
		}

		/// <summary>
		/// Gets the name of the property on the Component that this ComponentProperty applies to.
		/// </summary>
		public string PropertyName {
			get { return _PropertyName; }
		}

		/// <summary>
		/// Gets the ValueGenerator that's being used to generate the value to assign this property.
		/// </summary>
		public PropertyValueGenerator ValueGenerator {
			get { return _ValueGenerator; }
		}

		/// <summary>
		/// Applies the value resulting from this property onto the given Component instance.
		/// </summary>
		public void ApplyValue(Component Component) {
			PropertyInfo Property = Component.GetType().GetProperty(PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			if(Property == null)
				throw new KeyNotFoundException("Unable to find a property named '" + PropertyName + "' on Component of type '" + Component.GetType().Name + "'.");
			object Value = ValueGenerator.GetValue(Component, Property, _GeneratorArguments.Select(c=> c.GetValue()).ToArray());
			object Converted = Convert.ChangeType(Value, Property.PropertyType);
			Property.SetValue(Component, Converted, null);
		}

		public ComponentProperty(string ComponentName, string PropertyName, PropertyValueGenerator ValueGenerator, ComponentArgument[] GeneratorArguments) {
			this._ComponentName = ComponentName;
			this._PropertyName = PropertyName;
			this._GeneratorArguments = GeneratorArguments;
			this._ValueGenerator = ValueGenerator;
		}

		public override string ToString() {
			return "ComponentProperty: " + ComponentName + "." + PropertyName;
		}

		string _ComponentName;
		string _PropertyName;
		PropertyValueGenerator _ValueGenerator;
		ComponentArgument[] _GeneratorArguments;
	}
}
