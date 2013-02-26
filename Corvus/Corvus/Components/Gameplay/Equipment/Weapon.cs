using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components.Gameplay.Equipment
{
    //TODO: Possibly rename this class so it can be applied to all equipment. (Assuming we want to implement armor, accessories, etc.)
    //TODO: Add some sort of skill management. 
    //TODO: Maybe find a way for this weapon to have a effect (EX: Life steal, poison, etc)
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

        public Attributes Attributes { get; set; }
    }
}
