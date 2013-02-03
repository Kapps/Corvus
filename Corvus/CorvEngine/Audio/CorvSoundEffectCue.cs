using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

/*
 * Needs some more work. The sound doesn't update when the objects move. Meaning the sound will sound the same even though
 * the object move away. Tough to explain in text. 
 */
namespace CorvEngine.Audio
{
    /// <summary>
    /// A class for handling sound effects. Supports 3D Attenuation. 
    /// </summary>
    public class CorvSoundEffectCue : CorvCue
    {
        private AudioEmitter _Emitter;
        private AudioListener _Listener;

        /// <summary>
        /// Creates a new instance of CorvSoundEffectCue without any attenuation.
        /// </summary>
        public CorvSoundEffectCue(Cue cue)
            : base(cue)
        { }

        /// <summary>
        /// Creates a new instance of CorvSoundEffectCue with attenuation.
        /// </summary>
        public CorvSoundEffectCue(Cue cue, Vector2 listenerPosition, Vector2 emitterPosition)
            : base(cue)
        {
            this._Listener = new AudioListener();
            this._Listener.Position = new Vector3(listenerPosition, 0);
            this._Emitter = new AudioEmitter();
            this._Emitter.Position = new Vector3(emitterPosition, 0);

            Cue.Apply3D(this._Listener, this._Emitter);
        }

        /// <summary>
        /// Plays the Cue.
        /// </summary>
        public void Play()
        {
            Cue.Play();
        }

    }
}
