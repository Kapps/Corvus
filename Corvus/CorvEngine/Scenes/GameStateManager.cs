using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Scenes {
	/// <summary>
	/// Provides a handler used to manage GameStates.
	/// </summary>
	public class GameStateManager : DrawableGameComponent {

		private enum TransitionState : byte {
			NoTransition,
			StartTransition,
			MidTransition
		}

		private LinkedList<GameState> States = new LinkedList<GameState>();
		private GameStateTransition _Transition;

		private bool IsTransitioning;
		private TimeSpan TransitionTimeElapsed = TimeSpan.FromTicks(0);
		private TransitionMode TransitionMode;
		// Stores the last popped node for the purposes of transition.
		// To push a state, just render everything up until the new node, then render the new node.
		// To pop a state, store the node being popped, render everything, then render only the popped node.
		//private RenderTarget2D PoppedStateRender;
		//private RenderTarget2D CurrentRender;
		//private RenderTarget2D TransitionRender;
		//private RenderTarget2D PoppedStateTransitionRender;

		// So we know when to update CurrentRender's boundaries (or rather, recreate it).
		//private int CachedWidth, CachedHeight;


		/// <summary>
		/// Creates a new  GameStateManager. This is done by the engine.
		/// </summary>
		public GameStateManager(Game Game) : base(Game) {
			//this.CachedWidth = this.CachedHeight = -1;
			this.Transition = new AlphaTransition();
		}

		/// <summary>
		/// Gets or sets the transition to use when switching between GameStates.
		/// </summary>
		public GameStateTransition Transition {
			get { return _Transition; }
			set { _Transition = value; }
		}

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <returns></returns>
        public GameState GetCurrentState() {
            return this.States.Last();
        }

		/// <summary>
		/// Pushes the specified GameState to the top of the stack.
		/// </summary>
		public void PushState(GameState State) {
			this.States.AddLast(State);
			BeginTransition(TransitionMode.Pushed);
		}

		/// <summary>
		/// Clears off the current GameState, activating the previous state and returning the state that was removed.
		/// </summary>
		public GameState PopState() {
			var Last = this.States.Last;
			this.States.RemoveLast();
			BeginTransition(TransitionMode.Popped);
			return Last.Value;
		}

		/// <summary>
		/// Manages most rendering for the engine, drawing any currently active states.
		/// </summary>
		public override void Draw(GameTime gameTime) {
			/*int NewWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
			int NewHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
			if(NewWidth != CachedWidth || NewHeight != CachedHeight) {
				// We can probably just create 2/3 of these on demand, but eh.
				RecreateRenderTarget(ref CurrentRender);
				RecreateRenderTarget(ref TransitionRender);
				RecreateRenderTarget(ref PoppedStateRender);
			}

			if(this.IsTransitioning) {
				var PreviousTargets = GraphicsDevice.GetRenderTargets();
				RenderTarget2D From, To;
				if(TransitionMode == Scenes.TransitionMode.Pushed) {
					// If we're doing a transition for a push, we simply render up to BlocksDraw from Head-1 for from, then just Head alone for to.
					var Last = States.Last;

					// Draw everything except tail.
					SetRenderTarget(CurrentRender);
					if(States.Count > 1) // If nothing, just leave it transparent.
						RenderReversed(Last.Previous, gameTime);

					// Then, draw the tail node.
					SetRenderTarget(TransitionRender);
					RenderComponents(Last.Value, gameTime);

					From = CurrentRender;
					To = TransitionRender;
				} else {
					// If we're doing a transition for a pop, we have to use our stored TransitionRender (which gets copied from PoppedStateRender upon a Pop transition) as a from, and everything else as a to. Should work thanks to alpha.

					// We already have our From, so just do our To.
					// To is just a normal render.
					SetRenderTarget(CurrentRender);
					if(States.Count > 0)
						RenderReversed(States.Last, gameTime);

					From = PoppedStateTransitionRender;
					To = CurrentRender;
				}

				GraphicsDevice.SetRenderTargets(PreviousTargets);
			} else {
				// No transition; just a basic render.
				if(States.Count > 0)
					RenderReversed(States.Last, gameTime);
			}

			// Lastly, always keep our PoppedStateRender up to date.
			SetRenderTarget(PoppedStateRender);
			if(States.Count > 0)
				RenderComponents(States.Last.Value, gameTime);
			*/
			//GraphicsDevice.Clear(Color.CornflowerBlue);
			RenderReversed(States.Last, gameTime);
			base.Draw(gameTime);
		}

		/// <summary>
		/// Manages updating of active GameStates.
		/// </summary>
		public override void Update(GameTime gameTime) {
			// While this could be optimized in many ways, it probably doesn't need to be. Same for Draw.
			var UpdateHead = FirstReversed(c => c.BlocksUpdate);
			for(var Node = UpdateHead; Node != null; Node = Node.Next) {
				foreach(var Component in Node.Value.Components.OrderBy(c => c.UpdateOrder)) {
					if(Component.Enabled)
						((IUpdateable)Component).Update(gameTime);
				}
			}
			base.Update(gameTime);
		}

		private LinkedListNode<GameState> FirstReversed(Func<GameState, bool> fun, LinkedListNode<GameState> Tail = null) {
			if(Tail == null)
				Tail = this.States.Last;
			for(var Node = Tail; Node != null; Node = Node.Previous)
				if(fun(Node.Value))
					return Node;
			return States.First;
		}

		private void BeginTransition(TransitionMode Mode) {
			if(IsTransitioning)
				FinishTransition();
			this.IsTransitioning = true;
			this.TransitionTimeElapsed = TimeSpan.FromTicks(0);
			this.TransitionMode = Mode;
			// Should probably copy to a texture, but we'll just use RenderTarget and prevent dispose instead.
			//this.PoppedStateTransitionRender = PoppedStateRender;
			//RecreateRenderTarget(ref PoppedStateRender, false);
		}

		private void FinishTransition() {
			this.IsTransitioning = false;
			this.TransitionTimeElapsed = TimeSpan.FromTicks(0);
			this.TransitionMode = 0;
			//if(this.PoppedStateTransitionRender != null && !this.PoppedStateTransitionRender.IsDisposed)
			//	this.PoppedStateTransitionRender.Dispose();
		}

		private void SetRenderTarget(RenderTarget2D Target) {
			GraphicsDevice.SetRenderTarget(Target);
			GraphicsDevice.Clear(Color.Transparent);
		}

		private void RenderReversed(LinkedListNode<GameState> Tail, GameTime Time) {
			var RenderHead = FirstReversed(c => c.BlocksDraw);
			for(var Node = RenderHead; Node != null; Node = Node.Next) {
				RenderComponents(Node.Value, Time);
			}
		}

		private void RecreateRenderTarget(ref RenderTarget2D Target, bool Dispose = true) {
			if(Target != null && !Target.IsDisposed && Dispose)
				Target.Dispose();
			Target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
		}

		private void RenderComponents(GameState State, GameTime Time) {
			foreach(var Component in State.Components.OrderBy(c => c.DrawOrder)) {
				if(Component.Visible)
					((IDrawable)Component).Draw(Time);
			}
		}
	}
}
