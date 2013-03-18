using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corvus.Components.Gameplay
{
    [Flags]
    public enum Elements
    {
        None = 0,
        Physical = 1,
        Fire = 2,
        Water = 4,
        Wind = 8,
        Earth = 16,
        /// <summary>
        /// All means Fire, Water, Wind, and Earth. Should only be used for Resistance.
        /// </summary>
        All = 32
    }
}
