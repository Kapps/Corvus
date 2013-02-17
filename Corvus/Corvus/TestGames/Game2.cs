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
using CorvEngine.Graphics;

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

            Menu menu = new Menu(Content.Load<SpriteFont>(FONT_STRING), Color.Yellow);
            menu.Margin = new Thickness(20f, 20f, 0, 0);
            menu.AddItem("Show MessageBox1", (sender, e) =>
            {
                box.Show();
                controlManager.Push(box, ControlManager.QueueCommand.Add);
                menu.IsEnabled = false;
            });
            menu.AddItem("Play Song1", (sender, e) =>
            {
                AudioManager.PlayMusic("TestSong1", 1f, AudioTransitionStates.CrossFade);
            });
            menu.AddItem("Play Song2", (sender, e) =>
            {
                AudioManager.PlayMusic("TestSong2", 1f, AudioTransitionStates.CrossFade);
            });
            menu.AddItem("Play sound effect", (sender, e) =>
            {
                AudioManager.PlaySoundEffect("Select");
            });
            menu.AddItem("Play sound effect from the right!", (sender, e) =>
            {
                AudioManager.PlaySoundEffect("Select", new Vector2(0), new Vector2(200f, 0f));
            });
            menu.AddItem("Quit", (sender, e) =>
            {
                this.Exit();
            });

            mainPanel.Add(menu);

            controlManager.Add(mainPanel);
            //controlManager.SetFocus(button1);
            controlManager.SetFocus();//menu.GetButton(0));

            box = new MessageBox("This is a message box. NOTICE HOW YOU CANT FOCUS ON THE OTHER ELEMENTS HAHAHAHAHAHA. NOW to make it actually look nice......",
                                            Content.Load<SpriteFont>(FONT_STRING), Content.Load<Texture2D>(IMAGE_STRING));
            box.Position = new Vector2(200f);
            box.Selected += (sender, e) =>
            {
                box.Hide();
                controlManager.Push(box, ControlManager.QueueCommand.Remove);
                menu.IsEnabled = true;
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
