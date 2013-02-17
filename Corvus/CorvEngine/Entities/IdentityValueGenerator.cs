using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a PropertyValueGenerator that simply returns the argument that was passed in.
	/// </summary>
	public class IdentityValueGenerator : PropertyValueGenerator {
		public override string Name {
			get { return "Identity"; }
		}

		public override object GetValue(object Instance, object[] Arguments) {
			if(Arguments.Length != 1)
				throw new ArgumentException("Expected only a single argument to the Identity generator.");
			return Arguments[0];
		}
	}
}
