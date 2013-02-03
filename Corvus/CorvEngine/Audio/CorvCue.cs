using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CorvEngine.Audio
{
    /// <summary>
    /// Base Cue class. 
    /// </summary>
    public class CorvCue : IDisposable
    {
        private Cue _Cue;
        private bool _Disposed;

        /// <summary>
        /// Gets the Cue.
        /// </summary>
        public Cue Cue { get { return _Cue; } }

        /// <summary>
        /// Creates a new CorvCue.
        /// </summary>
        public CorvCue(Cue cue)
        {
            this._Cue = cue;
        }

        /// <summary>
        /// Sets the variable created in XACT.
        /// </summary>
        public void SetVariable(string variable, float value)
        {
            _Cue.SetVariable(variable, value);
        }

        /// <summary>
        /// Gets the variable created in XACT.
        /// </summary>
        public void GetVariable(string variable)
        {
            _Cue.GetVariable(variable);
        }

        /// <summary>
        /// Garbage Collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Garbage collection.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    if (_Cue != null)
                    {
                        _Cue.Dispose();
                        _Cue = null;
                    }
                }

                this._Disposed = true;
            }
        }
    }
}
