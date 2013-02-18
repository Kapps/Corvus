using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a PropertyValueGenerator that loads a Sprite through the Content pipeline.
	/// IMPORTANT: At the moment, this class uses the global Content loader, which will cause the resulting sprite to never be unloaded.
	/// </summary>
	public class SpriteValueGenerator : PropertyValueGenerator {

		public override string Name {
			get { return "Sprite"; }
		}

		public override object GetValue(object Instance, PropertyInfo Property, object[] Arguments) {
			if(Arguments.Length != 1)
				throw new ArgumentException("Expected a single argument to the SpriteValueGenerator, being the name of the Sprite to load.");
			return CorvBase.Instance.GlobalContent.LoadSprite(Arguments[0].ToString());
		}
	}
}
