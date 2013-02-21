using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Components.Blueprints {
	/// <summary>
	/// Provides a blueprint class that can be used to construct an Entity from a predefined set of Components properties.
	/// </summary>
	public class EntityBlueprint {
		/**
		 * TODO:
		 *	First, we have a list of components (probably the type name, such as CorvEngine.Components.PhysicsComponent).
		 *	Next, we have a list of properties. Properties say what Component they're applied to, and what they do.
		 *		They're not just flat values though, they can be calls to a ValueGenerator.
		 *		For example, the value 'MaxHealth: Random(100, 150)' would call the RandomValueGenerator class to get a number between that.
		 *		If a property is defined afterwards, it replaces the existing one. This allows us to implement inheriting of other blueprints while replacing values.
		 *	Finally, make a ContentProcessor parse our format and return an EntityBlueprint.
		 *		But should it be a ContentProcessor? Perhaps it should just be done at runtime with a Reload function.
		 *		Reloading would not affect Entities that have already been created; only newly created ones.
		 *	Of course an EntityBlueprint needs a unique name that it can be referred to by.
		 *	
		 *  Possible format:
		 *  
		 *		WeakBat
		 *			HealthComponent:
		 *				MaxHealth: Random(30, 35)
		 *				CurrentHealth: Value(MaxHealth)
		 *			SpriteComponent:
		 *				Sprite: Content(/Sprites/WeakBat)
		 *				Color: Color(255, 255, 200, 255)
		 *			BatAIComponent:
		 *				Speed: 50
		 *		
		 *  
		 *		StrongBat : WeakBat
		 *			HealthComponent:
		 *				MaxHealth: Random(60, 75)
		 *			SpriteComponent:
		 *				Color: Color(200, 200, 255, 255)
		 *			-BatAIComponent
		 *			StrongBatAIComponent:
		 *				
		 *		
		 */

		
		/// <summary>
		/// Gets the name of this EntityBlueprint.
		/// This value is immutable.
		/// </summary>
		public string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Gets the names of the types of the components that should be created for Entities using this blueprint.
		/// </summary>
		public IEnumerable<ComponentBlueprint> Components {
			get { return _Components; }
		}

		/// <summary>
		/// Creates an Entity from this Blueprint.
		/// The size and position of the Entity will remain zero, as they are not set by EntityBlueprints.
		/// </summary>
		public Entity CreateEntity() {
			Entity Result = new Entity();
			foreach(var Component in this.Components) {
				var GeneratedComponent = Component.CreateComponent();
				Result.Components.Add(GeneratedComponent);
			}
			return Result;
		}

		/// <summary>
		/// Creates a new EntityBlueprint with the specified name and components, replacing any existing EntityBlueprint with that name.
		/// </summary>
		public static EntityBlueprint CreateBlueprint(string Name, IEnumerable<ComponentBlueprint> Components) {
			var Result = new EntityBlueprint() {
				_Name = Name,
				_Components = Components.ToList()
			};
			lock(AllBlueprints) {
				AllBlueprints[Result.Name] = Result;
			}
			return Result;
		}

		/// <summary>
		/// Returns the EntityBlueprint with the specified name, or null if no Blueprint with that name was found.
		/// </summary>
		public static EntityBlueprint GetBlueprint(string Name) {
			EntityBlueprint Result;
			if(!AllBlueprints.TryGetValue(Name, out Result))
				return null;
			return Result;
		}

		public override string ToString() {
			return this.Name + " (" + this._Components.Count + " Component(s))";
		}

		private string _Name;
		private List<ComponentBlueprint> _Components;

		private static Dictionary<string, EntityBlueprint> AllBlueprints = new Dictionary<string, EntityBlueprint>(StringComparer.InvariantCultureIgnoreCase);
	}
}
