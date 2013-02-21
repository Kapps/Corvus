using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CorvEngine.Components.Blueprints;

namespace CorvEngine.Components.Blueprints.Generators {
	/// <summary>
	/// Provides a generator that can be used to apply an operation to one or more arguments, returning a value to be assigned from them.
	/// All derived classes must have a parameterless constructor, but it can be private.
	/// </summary>
	public abstract class PropertyValueGenerator {

		/// <summary>
		/// Gets the name of this PropertyValueGenerator.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Applies this generator to get a value from the specified arguments on the specified object.
		/// The resulting value is then converted into the type expected for the property and assigned.
		/// This function does not need to handle conversion nor assignment itself.
		/// </summary>
		public abstract object GetValue(object Instance, PropertyInfo Property, object[] Arguments);

		/// <summary>
		/// Gets a PropertyValueGenerator with the given name, or null if no generator was found.
		/// This method is pure, returning the same PropertyValueGenerator instance each time for an equivalent Name.
		/// </summary>
		public static PropertyValueGenerator GetGenerator(string Name) {
			EnsureGeneratorsInitialized();
			return Generators[Name];
		}

		private static void EnsureGeneratorsInitialized() {
			lock(Generators) {
				if(!AreGeneratorsInitialized) {
					AreGeneratorsInitialized = true;
					foreach(var Assembly in AssemblyManager.GetAssembliesToSearch()) {
						foreach(var Type in Assembly.GetTypes()) {
							if(Type.IsSubclassOf(typeof(PropertyValueGenerator))) {
								var Generator = CreateInstance(Type);
								Generators.Add(Generator.Name, Generator);
							}
						}
					}
				}
			}
		}

		private static PropertyValueGenerator CreateInstance(Type type) {
			return (PropertyValueGenerator)Activator.CreateInstance(type);
		}

		private static bool AreGeneratorsInitialized = false;
		private static Dictionary<string, PropertyValueGenerator> Generators = new Dictionary<string, PropertyValueGenerator>();
	}
}
