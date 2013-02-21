using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Components.Blueprints.Generators {
	/// <summary>
	/// Provides a PropertyValueGenerator that simply returns the argument that was passed in.
	/// </summary>
	public class IdentityValueGenerator : PropertyValueGenerator {
		public override string Name {
			get { return "Identity"; }
		}

		public override object GetValue(object Instance, PropertyInfo Property, object[] Arguments) {
			if(Arguments.Length != 1)
				throw new ArgumentException("Expected only a single argument to the Identity generator.");
			return Arguments[0];
		}
	}
}
