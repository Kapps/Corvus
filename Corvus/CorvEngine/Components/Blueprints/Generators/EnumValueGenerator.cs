using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Entities.Blueprints;

namespace CorvEngine.Entities.Generators {
	/// <summary>
	/// Provides a ValueGenerator that returns an enum with a qualified name.
	/// </summary>
	public class EnumValueGenerator : PropertyValueGenerator {

		public override string Name {
			get { return "Enum"; }
		}

		public override object GetValue(object Instance, System.Reflection.PropertyInfo Property, object[] Arguments) {
			if(Arguments.Length < 2)
				throw new FormatException("Expected at least two arguments; the first the qualified name of the enum, the second and up the values to assign.");
			string QualifiedName = Arguments[0].ToString();
			Type EnumType = Type.GetType(QualifiedName, false, true);
			if(EnumType == null) {
				foreach(var Assembly in AssemblyManager.GetAssembliesToSearch()) {
					EnumType = Assembly.GetType(QualifiedName, false, true);
					if(EnumType != null)
						break;
				}
			}
			if(EnumType == null)
				throw new ArgumentException("Did not find type named '" + QualifiedName + "'. Valid Example: Enum(CorvEngine.Direction, Left)");
			int Value = 0;
			for(int i = 1; i < Arguments.Length; i++) {
				string FlagName = Arguments[i].ToString();
				object FlagValue = Enum.Parse(EnumType, FlagName, true);
				int IntVal = (int)Convert.ChangeType(FlagValue, typeof(int));
				Value |= IntVal;
			}
			return Enum.ToObject(EnumType, (int)Value);
		}
	}
}
