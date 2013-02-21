using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Components.Blueprints.Generators {
	/// <summary>
	/// Provides an implementation of PropertyValueGenerator that generates a random number between two given values, returning the result as a double.
	/// </summary>
	public class RandomValueGenerator : PropertyValueGenerator {

		private static Random Random = new Random();

		public override string Name {
			get { return "Random"; }
		}

		public override object GetValue(object Instance, PropertyInfo Property, object[] Arguments) {
			if(Arguments.Length != 2)
				throw new ArgumentException("The RandomValueGenerator takes two arguments: a minimum value, and a maximum value.");
			double Min = (double)Convert.ChangeType(Arguments[0], typeof(double));
			double Max = (double)Convert.ChangeType(Arguments[1], typeof(double));
			double Difference = Max - Min;
			lock(Random) {
				double Result = Random.NextDouble() * Difference + Min;
				return Result;
			}
		}
	}
}
