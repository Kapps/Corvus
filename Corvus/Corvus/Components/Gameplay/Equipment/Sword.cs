using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corvus.Components.Gameplay.Equipment
{
    public class Sword : Weapon
    {
        public override string Name { get { return "Sword"; } }
        public override string AnimationName { get { return "SwordAttack"; } }

    }
}
