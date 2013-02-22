using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components{
    /// <summary>
    /// Handles the damage events for this entity.
    /// </summary>
    public class DamageComponent : Component{
        private AttributesComponent AttributesComponent;
        private FloatingTextList _FloatingTexts = new FloatingTextList();

        /// <summary>
        /// Applies damage with only normal rule.
        /// </summary>
        public void TakeDamage(float incomingDamage){
            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, incomingDamage);
            AttributesComponent.CurrentHealth -= damageTaken;

            _FloatingTexts.AddFloatingTexts(damageTaken, Color.White);
        }

        /// <summary>
        /// Applies damage, with the normal rules, based on the attacker's attributes.
        /// </summary>
        public void TakeDamage(AttributesComponent attacker){
            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, attacker.Attack);
            float criticalMultiplier = CriticalDamageChance(attacker.CritChance, attacker.CritDamage);
            float overallDamage = damageTaken * criticalMultiplier;
            AttributesComponent.CurrentHealth -= overallDamage;

            if (criticalMultiplier != 1)
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.Orange);
            else
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.White);
        }

        private float NormalDamageFormula(float myDefense, float incomingDamage){
            return Math.Max(incomingDamage - (myDefense * 0.70f), 1);
        }

        private float CriticalDamageChance(float critChance, float critDamage){
            Random rand = new Random();
            return (rand.NextDouble() <= critChance) ? critDamage : 1f;
        }

        protected override void OnInitialize(){
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
        }

        protected override void OnUpdate(GameTime Time){
            base.OnUpdate(Time);
            if (_FloatingTexts.Count == 0)
                return;
            var width = Parent.Size.X;
            var position = Parent.Position + new Vector2(width / 2, 0);
            var ToScreen = Camera.Active.ScreenToWorld(position);
            _FloatingTexts.Update(Time, ToScreen);
        }

        protected override void OnDraw(){
            base.OnDraw();
            if (_FloatingTexts.Count == 0)
                return;
            _FloatingTexts.Draw();
        }
    }

    //TODO: Refactor. Or let Onion do it since i'm not sure how he wants things.

    /// <summary>
    /// A collection of FloatingText. 
    /// </summary>
    public class FloatingTextList : List<FloatingText>
    {
        private SpriteFont _Font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/DamageFont");

        public void AddFloatingTexts(float value, Color color)
        {
            string text = Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString();
            this.Add(new FloatingText(text, _Font.MeasureString(text), color));
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            foreach (FloatingText dt in this.Reverse<FloatingText>())
            {
                dt.Update(gameTime, position);
                if (dt.IsFinished)
                    this.Remove(dt);
            }
        }

        public void Draw()
        {
            foreach (FloatingText dt in this)
                dt.Draw(_Font);
        }
    }


    /// <summary>
    /// A class to display the damage value.
    /// </summary>
    public class FloatingText
    {
        // TODO: This should be in it's own class so that other components can use it.

        /// <summary>
        /// Gets or sets the value to display.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the Text size.
        /// </summary>
        public Vector2 TextSize { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Gets a value determining whether this text is finished animating.
        /// </summary>
        public bool IsFinished { get; private set; }

        private Vector2 _Position { get; set; }
        private TimeSpan _Timer = new TimeSpan();
        private TimeSpan _Duration = TimeSpan.FromMilliseconds(450);
        private float _YIncrement = 0f;

        /// <summary>
        /// Creates a new instance of FloatingText.
        /// </summary>
        public FloatingText(string value, Vector2 textSize, Color textColor)
        {
            Value = value;
            TextSize = textSize;
            TextColor = textColor;
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            _Position = position + new Vector2(0, _YIncrement) - new Vector2(TextSize.X / 2, 0); //to center and make it move up
            _YIncrement -= 0.075f;
            _Timer += gameTime.ElapsedGameTime;
            if (_Timer >= _Duration)
                IsFinished = true;
        }

        public void Draw(SpriteFont font)
        {
            //TODO: Might want to make the font scale with the size of the object.
            CorvusGame.Instance.SpriteBatch.DrawString(font, Value, _Position, TextColor);
        }
    }
}