using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides a collection capable of storing SpriteFrames which may be accessed by either index or key.
	/// </summary>
	public class SpriteFrameCollection : KeyedCollection<string, SpriteFrame> {

		/// <summary>
		/// Creates a new SpriteFrameCollection.
		/// </summary>
		public SpriteFrameCollection() : base(StringComparer.InvariantCultureIgnoreCase) {
			
		}

		protected override string GetKeyForItem(SpriteFrame item) {
			return item.Name;
		}
	}
}
