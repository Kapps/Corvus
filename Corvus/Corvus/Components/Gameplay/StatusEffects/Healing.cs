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
        
        public override void Draw()
        {
            base.Draw(); 
            var ActiveFrame = _Sprite.ActiveAnimation.ActiveFrame.Frame;
            var SourceRect = ActiveFrame.Source;
            var position = Camera.Active.WorldToScreen(Entity.Position);
            var destinationRect = new Rectangle((int)(position.X + SourceRect.Width / 2), (int)(position.Y - Entity.Size.Y), SourceRect.Width, SourceRect.Height);
            CorvusGame.Instance.SpriteBatch.Draw(_Sprite.Texture, destinationRect, SourceRect, Color.White);
        
        }

        protected override void OnFirstOccurance()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float heal = Attributes.BaseValue;
            ac.CurrentHealth += heal;
            _FloatingTexts.AddFloatingTexts(heal, Color.Aqua);
        }

        protected override void OnTick()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float heal = ac.MaxHealth * Attributes.Intensity;
            ac.CurrentHealth += heal;
            _FloatingTexts.AddFloatingTexts(heal, Color.Aqua);
        }

        public Healing(Entity entity, StatusEffectProperties attributes) : base(entity, attributes) { }
    }
}
