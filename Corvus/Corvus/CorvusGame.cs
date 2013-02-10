using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;

namespace Corvus {
	/// <summary>
	/// Provides the main class for Corvus.
	/// </summary>
	public class CorvusGame : CorvBase {

		protected override void Initialize() {
			var TestState = new TestState();
			this.StateManager.PushState(TestState);
		}

	}
}
