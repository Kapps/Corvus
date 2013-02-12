using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides a collection of animations used for a sprite, accessible by name or index.
	/// </summary>
	public class SpriteAnimationCollection : KeyedCollection<string, SpriteAnimation> {

		protected override string GetKeyForItem(SpriteAnimation item) {
			return item.Name;
		}
	}
}
