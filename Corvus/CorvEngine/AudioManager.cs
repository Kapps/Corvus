using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using CorvEngine.Audio;


/*  Known Bugs:
 *      -If you switch songs in a middle of a transition, it won't play that new song.    
 *  Possible Bugs:
 *      -If one song is paused and another song is playing, if a third different song plays, not sure what will happen. Should figure that out eventually :p
 *  Possible Improvements:
 *      -Might want to decrease fadeout time.
 *  Drawbacks:
 *      -Must use .wav files which increase the size of our game by quite a bit. 
 */
namespace CorvEngine {
    /// <summary>
    /// Indicates which type of audio transition to use when changing songs.
    /// </summary>
    public enum AudioTransitionStates
    {
        /// <summary>
        /// While one song is fading out, the next song is fading in.
        /// </summary>
        CrossFade,

        /// <summary>
        /// The song fades out completely before the next one fades in.
        /// </summary>
        FadeOutFadeIn
    }

    /// <summary>
    /// A singleton class to handle soundeffects and music. 
    /// </summary>
    public class AudioManager : GameComponent
    {
        private static AudioManager _Instance = null;
        private AudioEngine _AudioEngine;
        private WaveBank _WaveBank;
        private SoundBank _SoundBank;
        private CorvMusicCue _MusicCue1;
        private CorvMusicCue _MusicCue2;
        private AudioTransitionStates _AudioTransitionState = AudioTransitionStates.CrossFade;

        /// <summary>
        /// Get the singleton instance of AudioManager.
        /// </summary>
        public static AudioManager Instance { get { return _Instance; } }

        /// <summary>
        /// Gets the AudioEngine.
        /// </summary>
        public AudioEngine AudioEngine { get { return _AudioEngine; } }

        /// <summary>
        /// Gets the WaveBank.
        /// </summary>
        public WaveBank WaveBank { get { return _WaveBank; } }

        /// <summary>
        /// Gets the SoundBank.
        /// </summary>
        public SoundBank SoundBank { get { return _SoundBank; } }

        /// <summary>
        /// Gets MusicCue1.
        /// </summary>
        public CorvMusicCue MusicCue1
        {
            get { return _MusicCue1; }
            private set { _MusicCue1 = value; }
        }

        /// <summary>
        /// Gets MusicCue2.
        /// </summary>
        public CorvMusicCue MusicCue2
        {
            get { return _MusicCue2; }
            private set { _MusicCue2 = value; }
        }

        /// <summary>
        /// Gets the AudioTransitionState.
        /// </summary>
        public AudioTransitionStates AudioTransitionState
        {
            get { return _AudioTransitionState; }
            private set { _AudioTransitionState = value; }
        }

        /// <summary>
        /// Creates a new instance of AudioManager with the specified game, xapfile, wavebankfile, and soundbankfile.
        /// </summary>
        public AudioManager(Game game, string xapFile, string waveBankFile, string soundBankFile)
            : base(game)
        {
            if (Interlocked.Exchange(ref _Instance, this) != null)
                throw new InvalidOperationException("Only one instance of AudioManager may exist at any time.");

            this._AudioEngine = new AudioEngine(xapFile);
            this._WaveBank = new WaveBank(this._AudioEngine, waveBankFile);
            this._SoundBank = new SoundBank(this._AudioEngine, soundBankFile);
        }

        /// <summary>
        /// Updates the AudioManager.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            _AudioEngine.Update();

            if (_MusicCue1 != null)
            {
                _MusicCue1.Update(gameTime);
                //if musicCue1 is stopped, do this.
                if (_MusicCue1.IsStopped)
                {
                    if (_AudioTransitionState == AudioTransitionStates.FadeOutFadeIn)
                        _MusicCue2.Play();

                    _MusicCue1.Dispose();
                    _MusicCue1 = null;
                }
                //If musicCue2 is paused, play this one
                else if (!_MusicCue1.IsPlaying && _MusicCue2.IsPaused)
                {
                    if (_AudioTransitionState == AudioTransitionStates.FadeOutFadeIn)
                        _MusicCue1.Play();
                }
            }

            if (_MusicCue2 != null)
            {
                _MusicCue2.Update(gameTime);
                //if musicCue2 is stopped, do this.
                if (_MusicCue2.IsStopped)
                {
                    if (_AudioTransitionState == AudioTransitionStates.FadeOutFadeIn)
                        _MusicCue1.Play();

                    _MusicCue2.Dispose();
                    _MusicCue2 = null;
                }
                //If musicCue1 is paused, play this one.
                else if (!_MusicCue2.IsPlaying && !_MusicCue2.IsStopped && _MusicCue1.IsPaused)
                {
                    if (_AudioTransitionState == AudioTransitionStates.FadeOutFadeIn)
                        _MusicCue2.Play();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        public static void PlaySoundEffect(string cueName)
        {
            //might want to add some sort of sound manager
            CorvSoundEffectCue cue = new CorvSoundEffectCue(_Instance.SoundBank.GetCue(cueName));
            cue.Play();
        }

        /// <summary>
        /// Plays a sound effect with 3D attenuation. The listener is the player(usually) and the emitter is what generates the sound.
        /// </summary>
        public static void PlaySoundEffect(string cueName, Vector2 listenerPosition, Vector2 emitterPosition)
        {
            CorvSoundEffectCue cue = new CorvSoundEffectCue(_Instance.SoundBank.GetCue(cueName), listenerPosition, emitterPosition);
            cue.Play();
        }

        /// <summary>
        /// Plays a new song with an optional fade duration and transition. Stops the current song if there is one playing.
        /// </summary>
        public static void PlayMusic(string musicName, float fadeDuration = 0, AudioTransitionStates transition = AudioTransitionStates.CrossFade)
        {
            if ((_Instance.MusicCue1 != null) && (_Instance.MusicCue1.Name == musicName) ||
                (_Instance.MusicCue2 != null) && (_Instance.MusicCue2.Name == musicName))
                return;

            _Instance.AudioTransitionState = transition;
            if (_Instance.MusicCue1 == null)
            {
                _Instance.MusicCue1 = new CorvMusicCue(_Instance.SoundBank.GetCue(musicName), fadeDuration);
                if (transition == AudioTransitionStates.CrossFade)
                    _Instance.MusicCue1.Play();
                if (_Instance.MusicCue2 != null && (!_Instance.MusicCue2.IsPaused && !_Instance.MusicCue2.IsPausing))
                    _Instance.MusicCue2.Stop();
            }
            else
            {
                _Instance.MusicCue2 = new CorvMusicCue(_Instance.SoundBank.GetCue(musicName), fadeDuration);
                if (transition == AudioTransitionStates.CrossFade)
                    _Instance.MusicCue2.Play();
                if (_Instance.MusicCue1 != null && (!_Instance.MusicCue1.IsPaused && !_Instance.MusicCue1.IsPausing))
                    _Instance.MusicCue1.Stop();
            }

            //deals with the initial case where nothing is playing.
            if (_Instance.MusicCue1 != null && !_Instance.MusicCue1.IsPlaying && _Instance.MusicCue2 == null)
                _Instance.MusicCue1.Play();
        }

        /// <summary>
        /// Pauses the current song and plays a new one.
        /// </summary>
        public static void PausePlay(string musicName, float fadeDuration = 0, AudioTransitionStates transition = AudioTransitionStates.CrossFade)
        {
            if (_Instance.MusicCue1 != null && _Instance.MusicCue1.IsPlaying)
                _Instance.MusicCue1.Pause();
            if (_Instance.MusicCue2 != null && _Instance.MusicCue2.IsPlaying)
                _Instance.MusicCue2.Pause();
            PlayMusic(musicName, fadeDuration, transition);
        }

        /// <summary>
        /// Resumes a paused song, if there is one.
        /// </summary>
        public static void ResumeMusic(AudioTransitionStates transition = AudioTransitionStates.CrossFade)
        {
            _Instance.AudioTransitionState = transition;
            if (_Instance.MusicCue1 != null && _Instance.MusicCue1.IsPaused)
            {
                if (transition == AudioTransitionStates.CrossFade)
                    _Instance.MusicCue1.Play();
                if (_Instance.MusicCue2 != null)
                    _Instance.MusicCue2.Stop();
            }
            if (_Instance.MusicCue2 != null && _Instance.MusicCue2.IsPaused)
            {
                if (transition == AudioTransitionStates.CrossFade)
                    _Instance.MusicCue2.Play();
                if (_Instance.MusicCue1 != null)
                    _Instance.MusicCue1.Stop();
            }
        }

        /// <summary>
        /// Stops all music.
        /// </summary>
        public static void StopMusic()
        {
            if (_Instance.MusicCue1 != null)
                _Instance.MusicCue1.Stop();

            if (_Instance.MusicCue2 != null)
                _Instance.MusicCue2.Stop();
        }

        /// <summary>
        /// Sets the volume for the music.
        /// </summary>
        /// <param name="value"></param>
        public static void SetMusicVolume(float value)
        {
            _Instance.AudioEngine.GetCategory("Music").SetVolume(MathHelper.Clamp(value, 0f, 2f));
        }

        /// <summary>
        /// Set a global variable created in XACT.
        /// </summary>
        public void SetGlobalVariable(string variable, float value)
        {
            _AudioEngine.SetGlobalVariable(variable, value);
        }

        /// <summary>
        /// Gets a global variable created in XACT.
        /// </summary>
        public float GetGlobalVariable(string variable)
        {
            return _AudioEngine.GetGlobalVariable(variable);
        }

        /// <summary>
        /// Garbage collection.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_MusicCue1 != null)
                    {
                        _MusicCue1.Dispose();
                        _MusicCue1 = null;
                    }

                    if (_MusicCue2 != null)
                    {
                        _MusicCue2.Dispose();
                        _MusicCue2 = null;
                    }

                    if (_SoundBank != null)
                    {
                        _SoundBank.Dispose();
                        _SoundBank = null;
                    }
                    if (_WaveBank != null)
                    {
                        _WaveBank.Dispose();
                        _WaveBank = null;
                    }
                    if (_AudioEngine != null)
                    {
                        _AudioEngine.Dispose();
                        _AudioEngine = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


    }
}
