using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Scenes {

	/// <summary>
	/// Indicates the reason that a transition occurred.
	/// </summary>
	public enum TransitionMode {
		/// <summary>
		/// A new GameState was pushed to the top of the stack.
		/// </summary>
		Pushed,
		/// <summary>
		/// A GameState was popped from the stack.
		/// </summary>
		Popped
	}

	/// <summary>
	/// Provides handling of transitioning between two GameStates.
	/// A transition is a global effect with knowledge about the state it's transitioning from, and the state it's transitioning to.
	/// Usually a GameStateTransition sets up a RenderTarget, then renders that with an overlay to display an effect.
	/// </summary>
	public abstract class GameStateTransition {
		/// <summary>
		/// Gets the total time that this transition should take.
		/// </summary>
		protected abstract TimeSpan TotalTime { get; }

		/// <summary>
		/// Gets an instance of GraphicsDevice that can be used for rendering.
		/// </summary>
		protected GraphicsDevice GraphicsDevice {
			get { return CorvBase.Instance.GraphicsDevice; }
		}

		/// <summary>
		/// Called before the GameStates being transitioned from are rendered.
		/// </summary>
		/// <param name="From">The current rendering of the GameState that was previously at the top of the stack. Can be null.</param>
		/// <param name="To">The current rendering of the GameState that is now going to be at the top of the stack. Can be null.</param>
		/// <param name="Mode">Indicates if To was pushed on to the stack, or if From was popped.</param>
		/// <param name="Elapsed">The amount of time that has elapsed since the start of the transition (not neccessarily real time).</param>
		protected abstract void RenderTransition(RenderTarget2D From, RenderTarget2D To, TransitionMode Mode, TimeSpan Elapsed);
	}
}
