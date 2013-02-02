using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Scenes {

	/// <summary>
	/// Represents a single state, or screen, in the game. For example, a main menu, the game itself, and credits screen would each be an individual GameState.
	/// GameStates may be pushed on top of each other, and may or may not cover the whole screen. All GameStates are paused when a new state is pushed.
	/// </summary>
	public abstract class GameState {
		private GameComponentCollection _Components = new GameComponentCollection();

		/// <summary>
		/// Indicates whether this GameState should prevent any previous states from updating.
		/// </summary>
		public abstract bool BlocksUpdate { get; }

		/// <summary>
		/// Indicates whether this GameState should prevent any previous states from rendering.
		/// </summary>
		public abstract bool BlocksDraw { get; }

		/// <summary>
		/// Gets all of the components present for this GameState.
		/// </summary>
		public IEnumerable<GameStateComponent> Components {
			get { return _Components.Select(c => (GameStateComponent)c); }
		}

		/// <summary>
		/// Adds the given component to this GameState.
		/// </summary>
		public void AddComponent(GameStateComponent Component) {
			if(_Components.Contains(Component))
				throw new ArgumentException("Component was already part of this GameState.");
			this._Components.Add(Component);
		}

		/// <summary>
		/// Removes the specified component from this collection.
		/// </summary>
		public void RemoveComponent(GameStateComponent Component) {
			bool result = this._Components.Remove(Component);
			if(!result)
				throw new ArgumentException("Component was not in this GameState.");
		}

		/// <summary>
		/// Returns the first GameStateComponent in this collection that can be casted to the specified type, or null if not found.
		/// </summary>
		public T GetComponent<T>() where T : GameStateComponent {
			foreach(var Component in _Components) {
				T Casted = Component as T;
				if(Casted != null)
					return Casted;
			}
			return null;
		}
	}
}
