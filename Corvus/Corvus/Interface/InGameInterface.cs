using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Components;
using Corvus.Interface.Controls;

namespace Corvus.Interface
{ 
    /// <summary>
    /// The ingame interface to display information such as health, mana, etc.
    /// </summary>
    public class InGameInterface : GameStateComponent
    {
        private List<PlayerUI> _PlayerUIs = new List<PlayerUI>();

        public InGameInterface(GameState gamestate)
            : base(gamestate)
        {
            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 700;
            CorvusGame.Instance.PlayerAdded += Instance_PlayerAdded;
            CorvusGame.Instance.PlayerRemoved += Instance_PlayerRemoved;
        }

        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time) { }

        protected override void OnDraw(Microsoft.Xna.Framework.GameTime Time)
        {
            if (_PlayerUIs.Count() == 0)
                return;
            var UIs = _PlayerUIs.Take(_PlayerUIs.Count());
            foreach (var p in UIs)
                p.Draw(Time);
        }

        void Instance_PlayerAdded(Player obj)
        {
            PlayerUI ui = new PlayerUI(obj);
            ui.Position = new Vector2(_PlayerUIs.Count() * (255f) + 5f, 5f);
            _PlayerUIs.Add(ui);
        }
        
        void Instance_PlayerRemoved(Player obj)
        {
            _PlayerUIs.Remove(_PlayerUIs.Where(p => p.ID == obj.Character.ID).SingleOrDefault());
        }

    }
}
