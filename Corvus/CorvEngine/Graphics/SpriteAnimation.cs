﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace CorvEngine.Graphics {
	/// <summary>
	/// Provides information about an animation used for a Sprite.
	/// </summary>
	public class SpriteAnimation {
		private int _Index;
		private List<SpriteAnimationFrame> _Frames;
		private bool _IsLooped;
		private TimeSpan _Elapsed;
		private bool _IsComplete;
		private string _Name;
		private bool _IsDefault;
		private float _SpeedModifier = 1;

		/// <summary>
		/// Gets an event called when the entire animation is completed.
		/// This method may be called multiple times for an animation with IsLooped set to true.
		/// </summary>
		public event EventHandler AnimationComplete;

		/// <summary>
		/// Gets the name of this animation.
		/// </summary>
		public string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Indicates whether this animation should be played in a loop.
		/// </summary>
		public bool IsLooped {
			get { return _IsLooped; }
		}

		/// <summary>
		/// Indicates whether this animation has been completed.
		/// For a looped animation, this is always false.
		/// </summary>
		public bool IsComplete {
			get { return _IsComplete; }
		}

		/// <summary>
		/// Indicates if this animation should be played by default when no other animations are playing.
		/// </summary>
		public bool IsDefault {
			get { return _IsDefault; }
		}

		/// <summary>
		/// Gets the currently active frame in this animation.
		/// </summary>
		public SpriteAnimationFrame ActiveFrame {
			get { return _Frames[_Index]; }
		}

		/// <summary>
		/// Returns the frames that are used for this animation.
		/// </summary>
		public IEnumerable<SpriteAnimationFrame> Frames {
			get { return _Frames; }
		}

		/// <summary>
		/// Gets or sets a value indicating how much this animation should be sped up or slowed down by.
		/// A value of 2 would indicate twice as fast, where as a value of 0.5 would indicate each frame takes twice as long.
		/// </summary>
		public float SpeedModifier {
			get { return _SpeedModifier; }
			set { _SpeedModifier = value; }
		}

		/// <summary>
		/// Advances the animation by the given period of time, provided that this animation is not yet complete.
		/// </summary>
		/// <param name="time"></param>
		public void AdvanceAnimation(TimeSpan time) {
			if(_IsComplete)
				return;
			_Elapsed += TimeSpan.FromTicks((long)(time.Ticks * this.SpeedModifier));
			if(ActiveFrame.Duration <= _Elapsed)
				AdvanceFrame();
		}

		/// <summary>
		/// Resets this animation to it's starting point.
		/// </summary>
		public void Reset() {
			_Index = 0;
			_Elapsed = TimeSpan.Zero;
			_IsComplete = false;
		}

		private void AdvanceFrame() {
			_Index++;
			if(_Index >= _Frames.Count) {
				if(_IsLooped) {
					_Index = 0;
				} else {
					_Index--; // Stay at the last frame.
					_IsComplete = true;
					if(AnimationComplete != null)
						AnimationComplete(this, new EventArgs());
				}
			}
			_Elapsed = TimeSpan.Zero;
		}

		public SpriteAnimation(string Name, bool IsLooped, bool IsDefault, IEnumerable<SpriteAnimationFrame> Frames) {
			this._Name = Name;
			this._IsLooped = IsLooped;
			this._IsDefault = IsDefault;
			this._Frames = Frames.ToList();
		}
	}
}
