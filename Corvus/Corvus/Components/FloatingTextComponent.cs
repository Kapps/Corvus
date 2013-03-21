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
    /// <summary>
    /// A component for drawing floating text beside the entity.
    /// </summary>
    public class FloatingTextComponent : Component
    {
        private FloatingTextList _FloatingTextList;

        /// <summary>
        /// Gets or sets the list of floating text.
        /// </summary>
        public FloatingTextList FloatingTextList
        { 
            get { return _FloatingTextList; }
            set { _FloatingTextList = value; }
        }

        /// <summary>
        /// Adds a floating text with the specified float value and color.
        /// </summary>
        public void Add(float value, Color color)
        {
            _FloatingTextList.AddFloatingTexts(value, color);
        }

        /// <summary>
        /// Adds a floating text with the specified string value and color.
        /// </summary>
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
