using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay.StatusEffects;

namespace Corvus.Components.Gameplay.Equipment
{
    //TODO: Add some sort of skill management. 
    /// <summary>
    /// The base class for a weapon.
    /// </summary>
    public abstract class Weapon
    {
        /// <summary>
        /// Gets the name of the weapon.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the animation name.
        /// </summary>
        public abstract string AnimationName { get; }

        /// <summary>
        /// The attributes from this weapon.
        /// </summary>
        public Attributes Attributes { get; set; }
    }
}
