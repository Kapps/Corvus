using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Components;
using Corvus.Components;
using Corvus.Components.Gameplay;

namespace Corvus.Interface.Controls
{
    /// <summary>
    /// A control to display the information of a single player.
    /// </summary>
    public class PlayerUI : BaseControl
    {
        /// <summary>
        /// Gets the ID this player ui is associated with.
        /// </summary>
        public int ID { get { return Player.Character.ID; } }

        private Player Player;
        private AttributesComponent AC;
        private EquipmentComponent EC;
        private PlayerControlComponent PCC;
        private ScoreComponent SC;

        private Texture2D MainUI = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PlayerUI");
        private Texture2D WeaponBG = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PlayerWeaponBg");
        private Texture2D HPMeter = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PlayerHealthMeter");
        private Texture2D MPMeter = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PlayerManaMeter");
        private Texture2D ExpBar = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PlayerExpBar");
        private Texture2D CoinImage = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PlayerCoin");

        private SpriteFont PlayerAttributesFont = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/PlayerAttributeFont");
        private SpriteFont PlayerNameFont = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/PlayerNameFont");
        private SpriteFont PlayerCoinFont = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/PlayerCoinsFont");

        public PlayerUI(Player player)
            :base(null)
        {
            this.Player = player;
			LoadDependencies();
			Player.CharacterChanged += (c) => LoadDependencies();
        }

		private void LoadDependencies() {
			this.AC = Player.Character.GetComponent<AttributesComponent>();
			this.EC = Player.Character.GetComponent<EquipmentComponent>();
			this.PCC = Player.Character.GetComponent<PlayerControlComponent>();
			this.SC = Player.Character.GetComponent<ScoreComponent>();
		}

        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime)
        {
            DrawWeaponArea(); //weapons
            DrawMainUI(); //Main UI
            DrawLevelArea();//text
            DrawMetersArea(); //Meters
            DrawCoinsArea();//Coin
            DrawExpBar();//exp
        }

        // TODO: Seriously need to clean this up.
		// For loops bro...
        private void DrawWeaponArea()
        {
            int weaponNumber = EC.Weapons.Count();
			string name;
			Texture2D texture;
			Color color;
			Vector2 pos;
			int i = 0;
			if(EC.Weapons.Count == 0)
				this.SpriteBatch.Draw(WeaponBG, GetRelativePosition(5f, 50f), new Rectangle(0, 0, 47, 48), Color.Black);
			else {
				i = EC.CurrentIndex;
				name = EC.Weapons[i].WeaponData.SystemName;
				texture = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Sprites/Equipment/" + name);
				color = DetermineColorByElement(EC.Weapons[i].Attributes.AttackingElements);
				this.SpriteBatch.Draw(WeaponBG, GetRelativePosition(5f, 50f), new Rectangle(0, 0, 47, 48), color);
				pos = GetRelativePosition(18f, 62f);
				this.SpriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, 24, 24), texture.Bounds, Color.White);
			}

            weaponNumber--;
            if (weaponNumber <= 0)
                this.SpriteBatch.Draw(WeaponBG, GetRelativePosition(54f, 50f), new Rectangle(0, 0, 44, 44), Color.Black);
            else
            {
                i = (i + 1 == EC.Weapons.Count()) ? 0 : i + 1;
                name = EC.Weapons[i].WeaponData.SystemName;
                texture = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Sprites/Equipment/" + name);
                color = DetermineColorByElement(EC.Weapons[i].Attributes.AttackingElements);
                this.SpriteBatch.Draw(WeaponBG, GetRelativePosition(54f, 50f), new Rectangle(0, 0, 44, 44), color);
                pos = GetRelativePosition(68f, 63f);
                this.SpriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, 20, 20), texture.Bounds, Color.White);
            }

            weaponNumber--;
            if (weaponNumber <= 0)
                this.SpriteBatch.Draw(WeaponBG, GetRelativePosition(98f, 50f), new Rectangle(0, 0, 44, 44), Color.Black);
            else
            {
                i = (i + 1 == EC.Weapons.Count()) ? 0 : i + 1;
                name = EC.Weapons[i].WeaponData.SystemName;
                texture = CorvusGame.Instance.GlobalContent.Load<Texture2D>("Sprites/Equipment/" + name);
                color = DetermineColorByElement(EC.Weapons[i].Attributes.AttackingElements);
                this.SpriteBatch.Draw(WeaponBG, GetRelativePosition(98f, 50f), new Rectangle(0, 0, 44, 44), color);
                pos = GetRelativePosition(113f, 63f);
                this.SpriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, 20, 20), texture.Bounds, Color.White);
            } 
        }

        private void DrawMainUI()
        {
            Color uiColor = Color.Lerp(Color.Violet, Color.LightSkyBlue, AC.CurrentHealth / AC.MaxHealth);
            this.SpriteBatch.Draw(MainUI, GetRelativePosition(0f, 0f), MainUI.Bounds, uiColor);
        }

        private void DrawLevelArea()
        {
            string currentLevel = PCC.Level.ToString();
            float offset = (currentLevel.Count() == 1) ? 0f : currentLevel.Count();
            this.SpriteBatch.DrawString(PlayerNameFont, currentLevel, GetRelativePosition(9f - offset, 11f), Color.White);
        }

        private void DrawMetersArea()
        {
            Color hpColor = Color.Lerp(Color.DarkRed, Color.Pink, AC.CurrentHealth / AC.MaxHealth);
            float hpWidth = 173f * AC.CurrentHealth / AC.MaxHealth;
            this.SpriteBatch.Draw(HPMeter, GetRelativePosition(72f, 7.5f), new Rectangle(0, 0, (int)hpWidth, 14), hpColor);
            string maxHP = ((int)AC.MaxHealth).ToString();
            this.SpriteBatch.DrawString(PlayerAttributesFont, ((int)AC.CurrentHealth).ToString(), GetRelativePosition(73.5f, 7.5f), Color.White);
            this.SpriteBatch.DrawString(PlayerAttributesFont, maxHP, GetRelativePosition(241f - PlayerAttributesFont.MeasureString(maxHP).X, 7.5f), Color.White);

            //Color mpColor = Color.Lerp(Color.MediumPurple, Color.SkyBlue, AC.CurrentMana / AC.MaxMana);
            float mpWidth = 173f * AC.CurrentMana / AC.MaxMana;
            this.SpriteBatch.Draw(MPMeter, GetRelativePosition(72f, 26.8f), new Rectangle(0, 0, (int)mpWidth, 14), Color.White);// mpColor);
            string maxMP = ((int)AC.MaxMana).ToString();
            this.SpriteBatch.DrawString(PlayerAttributesFont, ((int)AC.CurrentMana).ToString(), GetRelativePosition(73.5f, 26.8f), Color.White);
            this.SpriteBatch.DrawString(PlayerAttributesFont, maxMP, GetRelativePosition(241f - PlayerAttributesFont.MeasureString(maxMP).X, 26.8f), Color.White);
        }

        private void DrawCoinsArea()
        {
            this.SpriteBatch.Draw(CoinImage, GetRelativePosition(155f, 65f), CoinImage.Bounds, Color.White);
            string coins = SC.Coins.ToString();
            string display = "00000";
            display = display.Substring(coins.Count());
            this.SpriteBatch.DrawString(PlayerCoinFont, display + coins, GetRelativePosition(180f, 63f), Color.White);
        }

        private void DrawExpBar()
        {
            var expWidth = 250f * PCC.CurrentExperience / PCC.ExperienceForNextLevel;
            this.SpriteBatch.Draw(ExpBar, GetRelativePosition(0f, 106f), new Rectangle(0, 0, (int)expWidth, 3), Color.White);
        }

        private Color DetermineColorByElement(Elements element)
        {
            switch (element)
            {
                case Elements.Physical: return Color.LightGray;
                case Elements.Fire: return Color.LightCoral;
                case Elements.Water: return Color.LightBlue;
                case Elements.Wind: return Color.LightGreen;
                case Elements.Earth: return Color.RosyBrown;
                default: return Color.White;
            }
        }

        private Vector2 GetRelativePosition(float x, float y)
        {
            return new Vector2(this.Position.X + x, this.Position.Y + y);
        }
    }
}
