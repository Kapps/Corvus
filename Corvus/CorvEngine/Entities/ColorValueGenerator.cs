﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a value generator to generate a Color from either a name or an RGB(A) value.
	/// </summary>
	public class ColorValueGenerator : PropertyValueGenerator {
		public override string Name {
			get { return "Color"; }
		}

		public override object GetValue(object Instance, object[] Arguments) {
			if(Arguments.Length == 1) {
				string Name = Arguments[0].ToString();
				System.Drawing.Color SDColor = System.Drawing.Color.FromName(Name);
				return new Microsoft.Xna.Framework.Color(SDColor.R, SDColor.G, SDColor.B, SDColor.A);
			} else if(Arguments.Length == 3 || Arguments.Length == 4) {
				byte R = (byte)Convert.ChangeType(Arguments[0], typeof(byte));
				byte G = (byte)Convert.ChangeType(Arguments[1], typeof(byte));
				byte B = (byte)Convert.ChangeType(Arguments[2], typeof(byte));
				byte A = Arguments.Length == 4 ? (byte)Convert.ChangeType(Arguments[3], typeof(byte)) : (byte)255;
				return new Microsoft.Xna.Framework.Color(R, G, B, A);
			} else
				throw new ArgumentException("Expected argument to be either a named color, such as Blue, an RGB value, or an RGBA value.");
		}
	}
}
