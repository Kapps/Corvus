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
    /// A status effect that causes damage over time. Damage is calculated by: (MaxHealth * intensity + BaseEffect)
    /// </summary>
    public class Poison : Effect
    {
        public override string Name { get { return "Poison"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/testeffect1"; } }

        private TimeSpan _TickTimer = TimeSpan.FromSeconds(0); 

        public override void Update(GameTime gameTime, AttributesComponent ac, Vector2 position)
        {
            base.Update(gameTime, ac, position);
            _TickTimer += gameTime.ElapsedGameTime;
            if (_TickTimer >= TimeSpan.FromSeconds(1)) //apply every second
            {
                float damage = ac.MaxHealth * Intensity + BaseEffect;
                ac.CurrentHealth -= damage;
                _FloatingTexts.AddFloatingTexts(damage, Color.DarkViolet);
                _TickTimer = TimeSpan.Zero;
            }
        }

        public override void Draw()
        {
            var ActiveFrame = _EffectSprite.ActiveAnimation.ActiveFrame.Frame;
            var SourceRect = ActiveFrame.Source;
            var destinationRect = new Rectangle((int)(Position.X - EntitySize.X / 2 + SourceRect.Width / 2), (int)(Position.Y - EntitySize.Y), SourceRect.Width, SourceRect.Height);
            CorvusGame.Instance.SpriteBatch.Draw(_EffectSprite.Texture, destinationRect, SourceRect, Color.White);
            _FloatingTexts.Draw();
        }
    }
}
