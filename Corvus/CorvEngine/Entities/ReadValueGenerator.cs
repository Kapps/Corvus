using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a value generator that sets the value to a different property within that instance.
	/// </summary>
	public class ReadValueGenerator : PropertyValueGenerator {
		public override string Name {
			get { return "Read"; }
		}

		public override object GetValue(object Instance, System.Reflection.PropertyInfo Property, object[] Arguments) {
			if(Arguments.Length != 1 && Arguments.Length != 2)
				throw new ArgumentException("ReadValueGenerator takes in one or two arguments: The name of the property, and optionally an index within that property.");
			string ReadPropertyName = Arguments[0].ToString();
			int? ReadIndex = Arguments.Length == 2 ? (int?)(int)Convert.ChangeType(Arguments[1], typeof(int)) : null;
			var ReadProperty = Instance.GetType().GetProperty(ReadPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			object Value = ReadProperty.GetValue(Instance, ReadIndex == null ? null : new object[1] { ReadIndex });
			return Value;
		}
	}
}
