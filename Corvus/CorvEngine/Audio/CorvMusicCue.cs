using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CorvEngine.Audio
{
    /// <summary>
    /// Indicates which fade state the music cue is currently in.
    /// </summary>
    public enum FadeStates
    {
        /// <summary>
        /// Indicates there is no fading occuring.
        /// </summary>
        None,

        /// <summary>
        /// Indicates it is in the FadeIn state.
        /// </summary>
        FadeIn,

        /// <summary>
        /// Indicates it is in the FadeOut state and will pause the cue.
        /// </summary>
        FadeOutPause,

        /// <summary>
        /// Indicates it is in the FadeOut state and will stop playing.
        /// </summary>
        FadeOutStop
    }

    /// <summary>
    /// A class to manage a single cue (song). Adds fade in and fade out capabilities.
    /// </summary>
    public class CorvMusicCue : CorvCue
    {
        /// <summary>
        /// Used to handle fading. The actual variable is created in XACT.
        /// </summary>
        private const string MUSIC_VOLUME_VARIABLE = "MusicTrackVolume";

        private FadeStates _FadeState;
        private float _FadeDuration;
        private float _FadeTimer = 0f;
        private bool _IsPausing;
        private bool _IsStopped;

        /// <summary>
        /// Gets the name of the song.
        /// </summary>
        public string Name { get { return Cue.Name; } }

        /// <summary>
        /// Checks if the Cue is playing.
        /// </summary>
        public bool IsPlaying { get { return Cue.IsPlaying; } }

        /// <summary>
        /// Checks if the song is in the middle of pausing.
        /// </summary>
        public bool IsPausing { get { return _IsPausing; } }

        /// <summary>
        /// Checks if the cue has been paused.
        /// </summary>
        public bool IsPaused { get { return Cue.IsPaused; } }

        /// <summary>
        /// Checks if the Cue has stopped playing.
        /// </summary>
        public bool IsStopped { get { return _IsStopped; } }

        /// <summary>
        /// Creates a new CorvMusicCue with the given cue and fade duration.
        /// </summary>
        public CorvMusicCue(Cue cue, float fadeDuration)
            : base(cue)
        {
            this._FadeDuration = fadeDuration;
        }

        /// <summary>
        /// Updates the cue and it's fade states.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (_FadeState != FadeStates.None)
            {
                float elaspedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                FadeValue(elaspedTime);
            }
        }

        /// <summary>
        /// Plays the cue with a fade in effect. If it is pause, it will resume the cue.
        /// </summary>
        public void Play()
        {
            _FadeTimer = 0f;
            _FadeState = FadeStates.FadeIn;
            if (IsPaused)
                Cue.Resume();
            else
                Cue.Play();
        }

        /// <summary>
        /// Pauses the cue with fade out effect.
        /// </summary>
        public void Pause()
        {
            _IsPausing = true;
            _FadeTimer = 0f;
            _FadeState = FadeStates.FadeOutPause;
        }

        /// <summary>
        /// Stops the cue with a fade out and stop effect.
        /// </summary>
        public void Stop()
        {
            _FadeTimer = 0f;
            _FadeState = FadeStates.FadeOutStop;
        }

        /// <summary>
        /// Handles the FadeIn/FadeOut states.
        /// </summary>
        private void FadeValue(float elaspedTime)
        {
            _FadeTimer += elaspedTime;
            float musicVolume = MathHelper.Clamp(_FadeTimer / _FadeDuration, 0f, 1f);
            switch (_FadeState)
            {
                case FadeStates.FadeIn:
                    SetVariable(MUSIC_VOLUME_VARIABLE, musicVolume);
                    if (_FadeTimer >= _FadeDuration)
                        _FadeState = FadeStates.None;
                    break;
                case FadeStates.FadeOutPause:
                    SetVariable(MUSIC_VOLUME_VARIABLE, 1f - musicVolume);
                    if (_FadeTimer >= _FadeDuration)
                    {
                        _FadeState = FadeStates.None;
                        Cue.Pause();
                        _IsPausing = false;
                    }
                    break;
                case FadeStates.FadeOutStop:
                    SetVariable(MUSIC_VOLUME_VARIABLE, 1f - musicVolume);
                    if (_FadeTimer >= _FadeDuration)
                    {
                        Cue.Stop(AudioStopOptions.AsAuthored);
                        _FadeState = FadeStates.None;
                        _IsStopped = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
