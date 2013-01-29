using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Entities;

namespace Corvus {
	/// <summary>
	/// Provides an implementation of Player to be used in Corvus.
	/// </summary>
	class CorvusPlayer : Player {

		/// <summary>
		/// Gets the character that this player is currently controlling, if any.
		/// </summary>
		public Character Character { get; private set; }
	}
}
