using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Components;

namespace Corvus.Interface
{
    public class ArenaInterface : GameStateComponent
    {
        SpriteFont _font = CorvBase.Instance.GlobalContent.Load<SpriteFont>("Fonts/testfont");

        public ArenaInterface(GameState gamestate)
            : base(gamestate)
        {
            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 1000;
        }

        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time)
        {
            
        }

        protected override void OnDraw(Microsoft.Xna.Framework.GameTime Time)
        {
            var player = CorvBase.Instance.Players.First();

            if (player.Character.Scene != null)
            {
                var arenaSystem = player.Character.Scene.GetSystem<ArenaSystem>();

                if (arenaSystem != null)
                {
                    string wave = "Wave: " + arenaSystem.Wave;
                    CorvBase.Instance.SpriteBatch.DrawString(_font, wave, new Vector2(10, 120), Color.White);
                    string totalKills = "Kills: " + arenaSystem.TotalEntitiesKilled;
                    CorvBase.Instance.SpriteBatch.DrawString(_font, totalKills, new Vector2(10, 140), Color.White);
                    string totalKillsWave = "Kills (Wave): " + arenaSystem.TotalEntitiesKilledWave;
                    CorvBase.Instance.SpriteBatch.DrawString(_font, totalKillsWave, new Vector2(10, 160), Color.White);
                }
            }
        }
    }
}
