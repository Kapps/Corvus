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
    /// <summary>
    /// The ingame interface to display information such as health, mana, etc.
    /// </summary>
    public class InGameInterface : GameStateComponent
    {
        SpriteFont _font = CorvBase.Instance.GlobalContent.Load<SpriteFont>("Fonts/testfont");

        public InGameInterface(GameState gamestate)
            : base(gamestate)
        {
            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 700;
        }

        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time)
        {
            
        }

        protected override void OnDraw(Microsoft.Xna.Framework.GameTime Time)
        {
            //TODO: create a control that represents a single players interface thing.
            var player = CorvBase.Instance.Players.First();
            var ac = player.Character.GetComponent<Corvus.Components.AttributesComponent>();

            string hpstring = string.Format("HP: {0} / {1}", ((int)ac.CurrentHealth).ToString(), ((int)ac.MaxHealth).ToString());
            CorvBase.Instance.SpriteBatch.DrawString(_font, hpstring, new Vector2(10, 10), Color.Red);

            string mpstring = string.Format("Mana: {0} / {1}", ((int)ac.CurrentMana).ToString(), ((int)ac.MaxMana).ToString());
            CorvBase.Instance.SpriteBatch.DrawString(_font, mpstring, new Vector2(10, 30), Color.Blue);

            var ec = player.Character.GetComponent<Corvus.Components.EquipmentComponent>();
            string weaponstring = string.Format("Weapon: {0}", ec.CurrentWeapon.WeaponData.Name);
            CorvBase.Instance.SpriteBatch.DrawString(_font, weaponstring, new Vector2(10, 50), Color.White);

            var sc = player.Character.GetComponent<Corvus.Components.ScoreComponent>();
            string scoreString = "Coins: " + sc.Coins;
            CorvBase.Instance.SpriteBatch.DrawString(_font, scoreString, new Vector2(10, 70), Color.Gold);
        }
    }
}
