using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Components.Blueprints {
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
		/// Gets the argument that generates the value to assign to this property.
		/// </summary>
		public ComponentArgument Argument {
			get { return _GeneratorArgument; }
		}

		/// <summary>
		/// Applies the value resulting from this property onto the given Component instance.
		/// </summary>
		public void ApplyValue(Component Component) {
			//Component Component = Entity.Components[ComponentName];
			PropertyInfo Property = Component.GetType().GetProperty(PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			if(Property == null)
				throw new KeyNotFoundException("Unable to find a property named '" + PropertyName + "' on Component of type '" + Component.GetType().Name + "'.");
			object Value = Argument.GetValue(Component, Property);
			object Converted = Convert.ChangeType(Value, Property.PropertyType);
			Property.SetValue(Component, Converted, null);
		}

		/*public ComponentProperty(string ComponentName, string PropertyName, PropertyValueGenerator ValueGenerator, ComponentArgument[] GeneratorArguments) {
			this._ComponentName = ComponentName;
			this._PropertyName = PropertyName;
			this._GeneratorArguments = GeneratorArguments;
			this._ValueGenerator = ValueGenerator;
		}*/

		public ComponentProperty(string ComponentName, string PropertyName, ComponentArgument GeneratorArgument) {
			this._ComponentName = ComponentName;
			this._PropertyName = PropertyName;
			this._GeneratorArgument = GeneratorArgument;
		}

		public override string ToString() {
			return "ComponentProperty: " + ComponentName + "." + PropertyName;
		}

		string _ComponentName;
		string _PropertyName;
		ComponentArgument _GeneratorArgument;
	}
}
