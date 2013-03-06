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
    public class Healing : StatusEffect
    {
        public override string Name { get { return "Healing"; } }
        protected override string SpriteName { get { return "Sprites/StatusEffects/testeffect1"; } }

        private TimeSpan _TickTimer = TimeSpan.FromSeconds(0);
        private bool _IsFirstOccurance = false;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!_IsFirstOccurance)
            {
                var ac = Entity.GetComponent<AttributesComponent>();
                float heal = BaseValue;
                ac.CurrentHealth += heal;
                _FloatingTexts.AddFloatingTexts(heal, Color.Aqua);
                _IsFirstOccurance = true;
            }
            _TickTimer += gameTime.ElapsedGameTime;
            if (_TickTimer >= TimeSpan.FromSeconds(1))
            {
                var ac = Entity.GetComponent<AttributesComponent>();
                float heal = ac.MaxHealth * Intensity;
                ac.CurrentHealth += heal;
                _FloatingTexts.AddFloatingTexts(heal, Color.Aqua);
                _TickTimer = TimeSpan.Zero;
            }
        }
        
        public override void Draw()
        {
            base.Draw(); 
            var ActiveFrame = _Sprite.ActiveAnimation.ActiveFrame.Frame;
            var SourceRect = ActiveFrame.Source;
            var position = Camera.Active.ScreenToWorld(Entity.Position);
            var destinationRect = new Rectangle((int)(position.X + SourceRect.Width / 2), (int)(position.Y - Entity.Size.Y), SourceRect.Width, SourceRect.Height);
            CorvusGame.Instance.SpriteBatch.Draw(_Sprite.Texture, destinationRect, SourceRect, Color.White);
        
        }

        public Healing(Entity entity) : base(entity) { }
    }
}
