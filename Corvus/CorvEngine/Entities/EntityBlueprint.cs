using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a blueprint class that can be used to construct an Entity from a predefined set of Components properties.
	/// </summary>
	public class EntityBlueprint {
		/**
		 * TODO:
		 *	First, we have a list of components (probably the type name, such as CorvEngine.Entities.PhysicsComponent).
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
				
	}
}
