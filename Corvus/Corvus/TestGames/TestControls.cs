using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CorvEngine;
using CorvEngine.Controls;

namespace Corvus.TestGames
{
    public class TestControls: Microsoft.Xna.Framework.Game
    {
        private const string FONT_STRING = "Fonts/testfont";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public TestControls()
        {
            
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferHeight = 840;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.IsFullScreen = true;
            //graphics.ApplyChanges();
            Content.RootDirectory = "Content";
                       

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        StackPanel stack1;
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            stack1 = new StackPanel();

            TextBlock text1 = new TextBlock();
            text1.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            text1.Text = "HELLO BROLOS";
            text1.Foreground = Color.Orange;
            stack1.Add(text1);

            TextBlock text2 = new TextBlock();
            text2.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            text2.Text = "HELLO BROLOS IN GREEN";
            text2.Foreground = Color.Green;
            stack1.Add(text2);

            Image image1 = new Image();
            image1.Source = Content.Load<Texture2D>("Sprites/Player");
            stack1.Add(image1);

            StackPanel stack2 = new StackPanel();
            stack2.Orientation = Orientation.Horizontal;

            Image image2 = new Image();
            image2.Source = Content.Load<Texture2D>("Sprites/Player");
            stack2.Add(image2);

            StackPanel stack3 = new StackPanel();

            TextBlock text3 = new TextBlock();
            text3.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            text3.Text = "THIS IS PLAYER";
            text3.Foreground = Color.Red;
            stack3.Add(text3);

            TextBlock text4 = new TextBlock();
            text4.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            text4.Text = "TODO: Make the text start a new line when it extends too far.";
            text4.Foreground = Color.Blue;
            stack3.Add(text4);
            stack2.Add(stack3);
            stack1.Add(stack2);

            TextBlock text5 = new TextBlock();
            text5.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            text5.Text = "IF YOU SEE THIS ALEK, THAT SPRITE IS TIDUS FROM FINAL FANTASY 10";
            text5.Scale = 0.8f;
            stack1.Add(text5);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            stack1.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
