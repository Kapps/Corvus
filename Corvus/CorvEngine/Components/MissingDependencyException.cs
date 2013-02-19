using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// An exception thrown when a Component is missing a specific dependency.
	/// </summary>
	public class MissingDependencyException : Exception {

		/// <summary>
		/// Creates a new MissingDependencyException.
		/// </summary>
		/// <param name="Component">The component that is missing the dependency.</param>
		/// <param name="MissingType">The type of the component that was missing.</param>
		public MissingDependencyException(Component Component, Type MissingType)
			: base("Unable to find a Component of type '" + MissingType.Name + "' that '" + Component.Name + "' (" + Component.GetType().Name + ") had a dependency on.") {

		}
	}
}
