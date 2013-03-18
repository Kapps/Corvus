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

        public override void Draw()
        {
            base.Draw();
            var ActiveFrame = _Sprite.ActiveAnimation.ActiveFrame.Frame;
            var SourceRect = ActiveFrame.Source;
            var position = Camera.Active.WorldToScreen(Entity.Position);
            var destinationRect = new Rectangle((int)(position.X + SourceRect.Width / 2), (int)(position.Y - Entity.Size.Y), SourceRect.Width, SourceRect.Height);
            CorvusGame.Instance.SpriteBatch.Draw(_Sprite.Texture, destinationRect, SourceRect, Color.White);
        }

        protected override void OnFirstOccurance() { }

        protected override void OnTick()
        {
            var ac = Entity.GetComponent<AttributesComponent>();
            float damage = ac.MaxHealth * Attributes.Intensity + Attributes.BaseValue;
            ac.CurrentHealth -= damage;
            _FloatingTexts.AddFloatingTexts(damage, Color.DarkViolet);
        }

        //Unfortunately, need to specify this in order for StatusEffectsComponent to work properly.
        public Poison(Entity entity, StatusEffectProperties attributes) : base(entity, attributes) { }
    }
}
