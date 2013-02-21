using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Components.Blueprints.Generators {
	/// <summary>
	/// Provides a value generator that instantiates a collection with it's default type, if null, and then adds all arguments after being converted to the appropriate type.
	/// If the target property is both abstract and null, a list will be returned.
	/// </summary>
	public class CollectionValueGenerator : PropertyValueGenerator {

		public override string Name {
			get { return "Collection"; }
		}

		public override object GetValue(object Instance, PropertyInfo Property, object[] Arguments) {
			Type ElementType;
			object Collection = Property.GetValue(Instance, null);
			var CollectionType = Property.PropertyType;
			var GenericArgs = CollectionType.GetGenericArguments();
			if(GenericArgs.Any()) {
				if(GenericArgs.Length != 1)
					throw new NotSupportedException("Collections with more than one generic parameter are not yet supported.");
				ElementType = GenericArgs.First();
			} else
				ElementType = typeof(object);
			if(Collection == null) {
				Type TypeToInstantiate;
				if(ElementType.IsAbstract)
					TypeToInstantiate = typeof(List<>).MakeGenericType(ElementType);
				else
					TypeToInstantiate = Property.PropertyType;
				Collection = Activator.CreateInstance(TypeToInstantiate, true);
			}

			MethodInfo AddMethod = Collection.GetType().GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			foreach(var Argument in Arguments) {
				var Element = Convert.ChangeType(Argument, ElementType);
				AddMethod.Invoke(Collection, new object[] { Element });
			}
			return Collection;
		}
	}
}
