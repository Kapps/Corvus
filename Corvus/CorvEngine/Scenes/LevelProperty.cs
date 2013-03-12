using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Contains the properties for this level.
	/// </summary>
	public class LevelProperty {

		// TODO: This should be ComponentArgument integrated somehow,
		// so as to allow us to generate properties with more interesting values.

		/// <summary>
		/// The name of this property.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The value of this property.
		/// </summary>
		public string Value { get; set; }
		/// <summary>
		/// Creates a new MapProperty.
		/// </summary>
		public LevelProperty(string name, string value) {
			Name = name;
			Value = value;
		}
	}
}
