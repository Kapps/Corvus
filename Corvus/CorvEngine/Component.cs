using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine {
	// TODO: Implement this next.
	/// <summary>
	/// Provides a single system being used for a specific GameState.
	/// This class acts as a replacement to GameComponents, with CorvEngine features built in.
	/// </summary>
	public class Component : IGameComponent/*, IDrawable, IUpdateable*/ {
		private GameState _GameState;

		/// <summary>
		/// Indicates the GameState that this Component is applied to.
		/// </summary>
		public GameState GameState {
			get { return _GameState; }
		}

		/// <summary>
		/// Indicates whether this component should continue to be updated while paused.
		/// The default value is false.
		/// </summary>
		public virtual bool UpdateWhilePaused {
			get { return false; }
		}

		/// <summary>
		/// Explicitly defines the IGameComponent Initialize.
		/// This method should not be used by user code, and as such is sealed off.
		/// Instead, handle initialization in the constructor.
		/// </summary>
		void IGameComponent.Initialize() { 
			// We don't use this because GameComponent initialization is... odd, to say the least.
		}
	}
}
