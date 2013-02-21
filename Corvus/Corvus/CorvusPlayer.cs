using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Components;

namespace Corvus {
	/// <summary>
	/// Provides an implementation of Player to be used in Corvus.
	/// </summary>
	class CorvusPlayer : Player {
		public CorvusPlayer(Entity Character) : base(Character) { }
	}
}
