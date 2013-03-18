using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay;

namespace Corvus.Components
{
    public class FloatingTextComponent : Component
    {
        public FloatingTextList FloatingTextList { get { return _FloatingTextList; } }
        private FloatingTextList _FloatingTextList;

        public void Add(float value, Color color)
        {
            _FloatingTextList.AddFloatingTexts(value, color);
        }

        public void Add(string value, Color color)
        {
            _FloatingTextList.AddFloatingTexts(value, color);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _FloatingTextList = new FloatingTextList();
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);
            if (Camera.Active == null)
                return;
            var width = Parent.Size.X;
            var position = Parent.Position + new Vector2(width / 2, 0);
            var ToScreen = Camera.Active.WorldToScreen(position);
            _FloatingTextList.Update(Time, ToScreen);
        }

        protected override void OnDraw()
        {
            base.OnDraw();
            _FloatingTextList.Draw();
        }

    }
}
