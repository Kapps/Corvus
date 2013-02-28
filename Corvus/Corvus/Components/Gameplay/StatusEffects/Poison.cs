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
    /// <summary>
    /// A status effect that causes damage over time. Damage is calculated by: (MaxHealth * intensity + BaseValue)
    /// </summary>
    public class Poison : StatusEffect
    {
        public override string Name { get { return "Poison"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/testeffect1"; } }

        private TimeSpan _TickTimer = TimeSpan.FromSeconds(0);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _TickTimer += gameTime.ElapsedGameTime;
            if (_TickTimer >= TimeSpan.FromSeconds(1)) //apply every second
            {
                var ac = Entity.GetComponent<AttributesComponent>();
                float damage = ac.MaxHealth * Intensity + BaseValue;
                ac.CurrentHealth -= damage;
                _FloatingTexts.AddFloatingTexts(damage, Color.DarkViolet);
                _TickTimer = TimeSpan.Zero;
            }
        }

        public override void Draw()
        {
            var ActiveFrame = _Sprite.ActiveAnimation.ActiveFrame.Frame;
            var SourceRect = ActiveFrame.Source;
            var position = Camera.Active.ScreenToWorld(Entity.Position);
            var destinationRect = new Rectangle((int)(position.X + SourceRect.Width / 2), (int)(position.Y - Entity.Size.Y), SourceRect.Width, SourceRect.Height);
            CorvusGame.Instance.SpriteBatch.Draw(_Sprite.Texture, destinationRect, SourceRect, Color.White);
            _FloatingTexts.Draw();
        }

        //Unfortunately, need to specify this in order for StatusEffectsComponent to work properly.
        public Poison(Entity entity) : base(entity) { }
    }
}
