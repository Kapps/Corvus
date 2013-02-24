using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;
using CorvEngine.Components;

namespace Corvus.Components.Gameplay.StatusEffects
{
    //TODO: Maybe create a seperate class that handles all damage over time effects and another that
    //      handles all the one time only effects (Ex: strength down.)

    /// <summary>
    /// A status effect that causes damage over time.
    /// </summary>
    public class Poison : StatusEffect
    {
        public override string Name { get { return "Poison"; } }
        protected override string EffectName { get { return "Sprites/StatusEffects/testeffect1"; } }

        private TimeSpan _TickTimer = TimeSpan.FromSeconds(0); 

        public override void Update(GameTime gameTime, AttributesComponent ac, Vector2 position)
        {
            base.Update(gameTime, ac, position);
            _TickTimer += gameTime.ElapsedGameTime;
            if (_TickTimer >= TimeSpan.FromSeconds(1)) //apply every second
            {
                float damage = ac.MaxHealth * Intensity;
                ac.CurrentHealth -= damage;
                _FloatingTexts.AddFloatingTexts(damage, Color.DarkViolet);
                _TickTimer = TimeSpan.Zero;
            }
        }
    }
}
