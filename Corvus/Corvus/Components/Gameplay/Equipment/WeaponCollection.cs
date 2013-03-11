using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Corvus.Components.Gameplay.Equipment
{
    /// <summary>
    /// A Class to manage the weapons. The key is the weapon type.
    /// </summary>
    public class WeaponCollection : KeyedCollection<WeaponTypes, Weapon>
    {
        protected override WeaponTypes GetKeyForItem(Weapon item)
        {
            return item.WeaponData.WeaponType;
        }
    }
}
