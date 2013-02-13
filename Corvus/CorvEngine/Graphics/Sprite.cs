using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CorvEngine.Graphics {

	// TODO: Frames shouldn't be able to be added to.
	
	/// <summary>
	/// Represents a Sprite with animation frames available.
	/// </summary>
	public class Sprite {
		private static Random DefaultRandom = new Random();
		private Texture2D _Texture;
		private SpriteFrameCollection _Frames = new SpriteFrameCollection();
		private SpriteAnimationCollection _Animations;
		private SpriteAnimation _ActiveAnimation;

		/// <summary>
		/// Gets the texture that's used for this Sprite.
		/// </summary>
		public Texture2D Texture {
			get { return _Texture; }
		}

		/// <summary>
		/// Gets the frames present for this Sprite.
		/// </summary>
		public SpriteFrameCollection Frames {
			get { return _Frames; }
		}

		/// <summary>
		/// Gets the animations that are available for this sprite.
		/// </summary>
		public SpriteAnimationCollection Animations {
			get { return _Animations; }
		}

		/// <summary>
		/// Gets the currently active animation.
		/// </summary>
		public SpriteAnimation ActiveAnimation {
			get { return _ActiveAnimation; }
		}

		/// <summary>
		/// Stops the current animation, playing the specified animation instead.
		/// </summary>
		/// <param name="Name">The name of the animation to play.</param>
		public void PlayAnimation(string Name) {
			// TODO: Determine what to do if this is the active animation.
			// Chances are, that if it's loopable we carry on, and if not we reset the animation.
			var Animation = this.Animations[Name];
			Animation.Reset();
			this._ActiveAnimation = Animation;
		}

		/// <summary>
		/// Stops the currently playing animation, returning back to one of the default animations.
		/// </summary>
		public void StopAnimation() {
			this._ActiveAnimation = GetDefaultAnimation();
		}

		/// <summary>
		/// Returns the default animation for this sprite, or null if there is no default animation set.
		/// If there are multiple defaults, a random one is returned.
		/// </summary>
		/// <returns></returns>
		protected SpriteAnimation GetDefaultAnimation() {
			var Defaults = this._Animations.Where(c => c.IsDefault).ToArray();
			if(Defaults.Length == 0)
				return null;
			int Index;
			lock(DefaultRandom)
				Index = DefaultRandom.Next(0, Defaults.Length);
			return Defaults[Index];
		}

		/// <summary>
		/// Creates a new Sprite from the specified SpriteData instance.
		/// </summary>
		public Sprite(SpriteData Data) {
			SpriteFrameCollection Frames = new SpriteFrameCollection();
			foreach(var FrameData in Data.Frames) {
				var SpriteFrame = new SpriteFrame(FrameData.Name, FrameData.Source, this);
				Frames.Add(SpriteFrame);
			}
			SpriteAnimationCollection Animations = new SpriteAnimationCollection();
			foreach(var Animation in Data.Animations) {
				List<SpriteAnimationFrame> AnimationFrames = new List<SpriteAnimationFrame>();
				foreach(var Duration in Animation.FrameDurations) {
					var Frame = Frames[Duration.Key];
					SpriteAnimationFrame AnimationFrame = new SpriteAnimationFrame(Frame, Duration.Value);
					AnimationFrames.Add(AnimationFrame);
				}
				Animations.Add(new SpriteAnimation(Animation.Name, Animation.IsLooped, Animation.IsDefault, AnimationFrames));
			}

			this._Texture = Data.Texture;
			this._Frames = Frames;
			this._Animations = Animations;
			this._ActiveAnimation = GetDefaultAnimation();
		}

		/// <summary>
		/// Creates a new Sprite with the specified data.
		/// </summary>
		public Sprite(Texture2D Texture, SpriteFrameCollection Frames, SpriteAnimationCollection Animations) {
			this._Texture = Texture;
			this._Frames = Frames;
			this._Animations = Animations;
			this._ActiveAnimation = GetDefaultAnimation();
		}
	}
}
