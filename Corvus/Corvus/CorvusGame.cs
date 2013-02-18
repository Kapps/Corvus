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
			// TODO: Add your initialization logic here
			this.RegisterGlobalComponent(new AudioManager(this.Game, @"Content\Audio\RpgAudio.xgs", @"Content\Audio\Wave Bank.xwb", @"Content\Audio\Sound Bank.xsb"));
			var TestState = new TestState();
			this.StateManager.PushState(TestState);

			RegisterGlobalComponent(new DebugComponent());
		}		
	}
}
