using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Components;
using CorvEngine.Components.Blueprints;
using Microsoft.Xna.Framework;

namespace Corvus {
	/// <summary>
	/// Provides an implementation of Player to be used in Corvus.
	/// </summary>
	class CorvusPlayer : Player {
		public CorvusPlayer(Entity Character) : base(Character) { }

		/// <summary>
		/// Loads a new Entity that can be used for the player.
		/// </summary>
		public static Entity LoadPlayerEntity() {
			// TODO: Allow support for different 'classes' by just using different blueprints.
			var Blueprint = EntityBlueprint.GetBlueprint("Player");
			var PlayerEntity = Blueprint.CreateEntity();
			PlayerEntity.Size = new Vector2(36, 24);
			return PlayerEntity;
		}
	}
}
