using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components.Gameplay.StatusEffects
{
    /// <summary>
    /// A collection of StatusEffects.
    /// </summary>
    public class StatusEffectCollection : KeyedCollection<string, StatusEffect>
    {
        protected override string GetKeyForItem(StatusEffect item)
        {
            return item.Name;
        }

        /// <summary>
        /// We want to only add it if that type of status effect doesn't already exist. Otherwise update the existing one.
        /// </summary>
        protected override void InsertItem(int index, StatusEffect item)
        {
            if (!this.Contains(item.Name))
                base.InsertItem(index, item);
            else
            {
                //Update the existing one, if it's better.
                var effect = this[item.Name];
                if (effect.Intensity < item.Intensity)
                {
                    //new effect is better, apply it's properties
                    effect.Intensity = item.Intensity;
                    effect.Duration = item.Duration;
                }
                //Set the duration if it's longer.
                else if (effect.Duration < item.Duration)
                    effect.Duration = item.Duration;
                effect.ResetTimer();
            }
        }
    }
}
