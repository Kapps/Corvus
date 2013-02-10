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
using CorvEngine.Controls.UserControls;

namespace Corvus.TestGames
{
    public class Game2: Microsoft.Xna.Framework.Game
    {
        private const string FONT_STRING = "Fonts/testfont";
        private const string IMAGE_STRING = "Interface/testbg";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public Game2()
        {
            
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferHeight = 840;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.IsFullScreen = true;
            //graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            this.Components.Add(new InputHandler(this));
            this.Components.Add(new AudioManager(this, @"Content\Audio\RpgAudio.xgs", @"Content\Audio\Wave Bank.xwb", @"Content\Audio\Sound Bank.xsb"));

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        ControlManager controlManager;
        MessageBox box;
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            controlManager = new ControlManager();
            controlManager.UpdateAdd += new EventHandler<ControlManagerEventArgs>(controlManager_UpdateAdd);
            controlManager.UpdateRemove += new EventHandler<ControlManagerEventArgs>(controlManager_UpdateRemove);
            StackPanel mainPanel = new StackPanel();

            Label header = new Label();
            header.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            header.Foreground = Color.Purple;
            header.Text = "This is a label. Press Enter to select, Up/Down to navigate menu.";
            mainPanel.Add(header);

            TextBlock descriptor = new TextBlock();
            descriptor.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            descriptor.Scale = 0.8f;
            descriptor.Foreground = Color.Green;
            descriptor.Length = 80;
            descriptor.Text = "So this entire interface looks like crap. Gotta work on that. BUT the functionality is awesome! Although, there is bound to be thousands of bugs ;P";
            mainPanel.Add(descriptor);

            //
            StackPanel subPanel = new StackPanel();

            LinkButton button1 = new LinkButton();
            button1.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            button1.Foreground = Color.Yellow;
            button1.Text = "Show MessageBox1";
            button1.Selected += (sender, e) =>
            {
                box.Show();
                controlManager.Push(box, ControlManager.QueueCommand.Add);
                subPanel.IsEnabled = false;
            };
            subPanel.Add(button1);

            LinkButton button2 = new LinkButton();
            button2.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            button2.Foreground = Color.Yellow;
            button2.Text = "Play Song1";
            button2.Selected += (sender, e) =>
            {
                AudioManager.PlayMusic("TestSong1", 1f, AudioTransitionStates.CrossFade);
            };
            subPanel.Add(button2);

            LinkButton button3 = new LinkButton();
            button3.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            button3.Foreground = Color.Yellow;
            button3.Text = "Play Song2";
            button3.Selected += (sender, e) =>
            {
                AudioManager.PlayMusic("TestSong2", 1f, AudioTransitionStates.CrossFade);
            };
            subPanel.Add(button3);

            LinkButton button4 = new LinkButton();
            button4.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            button4.Foreground = Color.Yellow;
            button4.Text = "Play sound effect";
            button4.Selected += (sender, e) =>
            {
                AudioManager.PlaySoundEffect("Select");
            };
            subPanel.Add(button4);

            LinkButton button5 = new LinkButton();
            button5.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            button5.Foreground = Color.Yellow;
            button5.Text = "Play sound effect from the right!";
            button5.Selected += (sender, e) =>
            {
                AudioManager.PlaySoundEffect("Select", new Vector2(0), new Vector2(200f, 0f));
            };
            subPanel.Add(button5);

            LinkButton button6 = new LinkButton();
            button6.SpriteFont = Content.Load<SpriteFont>(FONT_STRING);
            button6.Foreground = Color.Yellow;
            button6.Text = "Quit";
            button6.Selected += (sender, e) =>
            {
                this.Exit();
            };
            subPanel.Add(button6);

            mainPanel.Add(subPanel);
            //
            controlManager.Add(mainPanel);
            controlManager.SetFocus(button1);


            box = new MessageBox("This is a message box. NOTICE HOW YOU CANT FOCUS ON THE OTHER ELEMENTS HAHAHAHAHAHA. NOW to make it actually look nice......",
                                            Content.Load<SpriteFont>(FONT_STRING), Content.Load<Texture2D>(IMAGE_STRING));
            box.Position = new Vector2(200f);
            box.Selected += (sender, e) =>
            {
                box.Hide();
                controlManager.Push(box, ControlManager.QueueCommand.Remove);
                subPanel.IsEnabled = true;
            };



        }

        void controlManager_UpdateAdd(object sender, ControlManagerEventArgs e)
        {
            if (e.Element == box)
                controlManager.SetFocus(box.Button);
        }

        void controlManager_UpdateRemove(object sender, ControlManagerEventArgs e)
        {
            controlManager.SetFocus();
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

            controlManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            controlManager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
