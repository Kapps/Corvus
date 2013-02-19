using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine {
	/// <summary>
	/// A global component used to trigger commands to be invoked every frame.
	/// </summary>
	public class FrameInvoker : GameComponent {

		public FrameInvoker() : base(CorvBase.Instance.Game) { }

		/// <summary>
		/// Registers the specified command to be invoked every frame.
		/// </summary>		
		public void RegisterCommand(Action Command) {
			Commands.Add(Command);
		}

		/// <summary>
		/// Removes the specified command, causing it to no longer be invoked on each frame.
		/// </summary>
		public void RemoveCommand(Action Command) {
			bool Result = Commands.Remove(Command);
			if(!Result)
				throw new KeyNotFoundException();
		}

		public override void Update(GameTime gameTime) {
			foreach(var Command in Commands)
				Command();
			base.Update(gameTime);
		}

		private List<Action> Commands = new List<Action>();
	}
}
