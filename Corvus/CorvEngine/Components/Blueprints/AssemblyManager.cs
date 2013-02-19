using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Entities.Blueprints {
	/// <summary>
	/// An internal helper used to provide a list of assemblies that contain references to CorvEngine.
	/// </summary>
	internal static class AssemblyManager {

		/// <summary>
		/// Gets all assemblies that have a reference to CorvEngine.
		/// </summary>
		public static IEnumerable<Assembly> GetAssembliesToSearch() {
			// TODO: Ideally we'd go over each loaded assembly and find the types that derive from PropertyValueGenerator.
			// Unfortunately, we can't do that while still continuing to use the .NET Client Profile.
			// So, for now we limit ourselves only to generators defined in the engine and the game running it. Which is fine for our purposes.
			// TODO: Allow all assemblies when not using the client profile using a #if.
			yield return Assembly.GetEntryAssembly();
			yield return Assembly.GetExecutingAssembly();
		}
	}
}
