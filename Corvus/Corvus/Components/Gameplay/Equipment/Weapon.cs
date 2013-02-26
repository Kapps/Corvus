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

        /// <summary>
        /// Gets or sets the strength modifier of this weapon.
        /// </summary>
        public float StrModifier
        {
            get { return _StrModifier; }
            set { _StrModifier = value; }
        }

        /// <summary>
        /// Gets or sets the dexterity modifier of this weapon.
        /// </summary>
        public float DexModifier
        {
            get { return _DexModifier; }
            set { _DexModifier = value; }
        }

        /// <summary>
        /// Gets or sets the intelligence modifier of this weapon.
        /// </summary>
        public float IntModifier
        {
            get { return _IntModifier; }
            set { _IntModifier = value; }
        }

        /// <summary>
        /// Gets or sets the critical chance modifier of this weapon.
        /// </summary>
        public float CritChanceModifier
        {
            get { return _CritChanceModifier; }
            set { _CritChanceModifier = value; }
        }

        /// <summary>
        /// Gets or sets the critical damage modifier of this weapon.
        /// </summary>
        public float CritDamageModifier
        {
            get { return _CritDamageModifier; }
            set { _CritDamageModifier = value; }
        }


        public float _StrModifier = 1f;
        public float _DexModifier = 1f;
        public float _IntModifier = 1f;
        public float _CritChanceModifier = 0f;
        public float _CritDamageModifier = 0f;
    }
}
