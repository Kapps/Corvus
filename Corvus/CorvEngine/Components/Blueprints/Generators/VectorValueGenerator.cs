using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Entities.Generators {
	/// <summary>
	/// A PropertyValueGenerator that returns a Vector(2|3|4) instance from input.
	/// </summary>
	public class VectorValueGenerator : PropertyValueGenerator {
		public override string Name {
			get { return "Vector"; }
		}

		public override object GetValue(object Instance, PropertyInfo Property, object[] Arguments) {
			float[] Values = Arguments.Select(c => (float)Convert.ChangeType(c, typeof(float))).ToArray();
			switch(Values.Length) {
				case 2:
					return new Vector2(Values[0], Values[1]);
				case 3:
					return new Vector3(Values[0], Values[1], Values[2]);
				case 4:
					return new Vector4(Values[0], Values[1], Values[2], Values[3]);
				default:
					throw new ArgumentException("Expected 2, 3, or 4 arguments to VectorValueGenerator.");
			}
		}
	}
}
