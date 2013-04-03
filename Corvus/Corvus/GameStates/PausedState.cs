using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Components.Blueprints;
using CorvEngine.Geometry;
using CorvEngine.Scenes;
using CorvEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Interface.Controls;
using Corvus.Interface;
using CorvEngine.Input;

namespace Corvus.GameStates
{
    /// <summary>
    /// A game state for the in game pause screen.
    /// </summary>
    public class PausedState : GameState
    {
        private ControlManagerComponent _ControlManager;

        /// <summary>
        /// Gets the control manager.
        /// </summary>
        public ControlManagerComponent ControlManager { get { return _ControlManager; } }

        public override bool BlocksDraw
        {
            get { return false; }
        }

        public override bool BlocksUpdate
        {
            get { return true; }
        }

        public PausedState()
            :base()
        {
            _ControlManager = new ControlManagerComponent(this);
            SpriteFont font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/MainMenuFont");


            Image bg = new Image(CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PausedBackgroundalt"));
            bg.Size = new Vector2(UIHelper.Viewport.Width, UIHelper.Viewport.Height);
            _ControlManager.AddControl(bg);

            Label lblpaused = new Label(font);
            lblpaused.Text = "Paused";
            lblpaused.Position = UIHelper.AlignControl(lblpaused.Size, HorizontalAlignment.Center, VerticalAlignment.Top)+ new Vector2(0, 20f);
            _ControlManager.AddControl(lblpaused);

            LinkButton continueBtn = new LinkButton(font);
            continueBtn.Text = "Continue";
            continueBtn.Selected += continueBtn_Selected;
            continueBtn.Position = UIHelper.AlignControl(continueBtn.Size, HorizontalAlignment.Center, VerticalAlignment.Center);
            _ControlManager.AddControl(continueBtn);

            LinkButton returnToMainMenuBtn = new LinkButton(font);
            returnToMainMenuBtn.Text = "Return to Main Menu";
            returnToMainMenuBtn.Selected += returnToMainMenuBtn_Selected;
            returnToMainMenuBtn.Position = UIHelper.AlignControl(returnToMainMenuBtn.Size, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0f, 50f);
            _ControlManager.AddControl(returnToMainMenuBtn);

            LinkButton exitGameBtn = new LinkButton(font);
            exitGameBtn.Text = "Exit Game";
            exitGameBtn.Selected += exitGameBtn_Selected;
            exitGameBtn.Position = UIHelper.AlignControl(exitGameBtn.Size, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0f, 100f);
            _ControlManager.AddControl(exitGameBtn);

            _ControlManager.SetFocus();
            this.AddComponent(_ControlManager);
        }

        void continueBtn_Selected(object sender, EventArgs e)
        {
            CorvusGame.Instance.StateManager.PopState();
        }


        void returnToMainMenuBtn_Selected(object sender, EventArgs e)
        {
            AudioManager.PlayMusic("Title1");
            AudioManager.SetMusicVolume(0.5f);
            //Hacky method. 
            CorvusGame.Instance.StateManager.PopState();
            CorvusGame.Instance.StateManager.PopState();
        }

        void exitGameBtn_Selected(object sender, EventArgs e)
        {
            CorvusGame.Instance.Exit();
        }
    }
}
