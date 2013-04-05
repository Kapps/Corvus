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
    /// A state to manage the main menu.
    /// </summary>
    public class MainMenuState : GameState
    {
        private ControlManagerComponent _ControlManager;

        /// <summary>
        /// Gets the control manager.
        /// </summary>
        public ControlManagerComponent ControlManager { get { return _ControlManager; } }

        public override bool BlocksUpdate
        {
            get { return true; }
        }

        public override bool BlocksDraw
        {
            get { return true; }
        }

        public MainMenuState()
            : base()
        {
            _ControlManager = new ControlManagerComponent(this);
            SpriteFont font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/MainMenuFont");

            Image bgImg = new Image(CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/TitleScreenBackground"));
            bgImg.Size = new Vector2(UIHelper.Viewport.Width, UIHelper.Viewport.Height);
            ControlManager.AddControl(bgImg);

            Image titleImg = new Image(CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/TitleText"));
            titleImg.Position = UIHelper.AlignControl(titleImg.Size, HorizontalAlignment.Center, VerticalAlignment.Top) + new Vector2(0, (UIHelper.Viewport.Height/2) * 0.1f);;
            ControlManager.AddControl(titleImg);

            var hackSize = new Vector2(175f, 50f); // a hack to align things.
            
            LinkButton continueBtn = new LinkButton(font);
            continueBtn.Text = "Continue";
            continueBtn.Color = Color.Black;
            continueBtn.IsVisible = false; //TODO: Will need to hide this if there is no saved state.
            continueBtn.IsEnabled = false;
            continueBtn.Position = UIHelper.AlignControl(hackSize, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0, 50f);
            continueBtn.Selected += continueBtn_Selected;
            ControlManager.AddControl(continueBtn);
            
            LinkButton newGameBtn = new LinkButton(font);
            newGameBtn.Text = "New Game";
            newGameBtn.Color = Color.Black;
            newGameBtn.Position = UIHelper.AlignControl(hackSize, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0, 100f);
            newGameBtn.Selected += newGameBtn_Selected;
            ControlManager.AddControl(newGameBtn);

            LinkButton arenaModeBtn = new LinkButton(font);
            arenaModeBtn.Text = "Arena Mode";
            arenaModeBtn.Color = Color.Black;
            arenaModeBtn.Position = UIHelper.AlignControl(hackSize, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0, 150f);
            arenaModeBtn.Selected += arenaModeBtn_Selected;
            ControlManager.AddControl(arenaModeBtn);

            LinkButton exitBtn = new LinkButton(font);
            exitBtn.Text = "Exit";
            exitBtn.Color = Color.Black;
            exitBtn.Position = UIHelper.AlignControl(hackSize, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0, 200f);
            exitBtn.Selected += exitBtn_Selected;
            ControlManager.AddControl(exitBtn);

#if DEBUG
            LinkButton testModeBtn = new LinkButton(font);
            testModeBtn.Text = "Test Mode";
            testModeBtn.Color = Color.Black;
            testModeBtn.IsEnabled = true;
            testModeBtn.Position = UIHelper.AlignControl(hackSize, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0, 250f);
            testModeBtn.Selected += testModeBtn_Selected;
            ControlManager.AddControl(testModeBtn);
#endif

            ControlManager.SetFocus();
            this.AddComponent(_ControlManager);
        }
        
        void continueBtn_Selected(object sender, EventArgs e)
        {
            
        }

        void newGameBtn_Selected(object sender, EventArgs e)
        {
            CorvusGame.Instance.SceneManager.ReloadScenes(false);
            CorvusGame.Instance.SceneManager.ChangeScene("Tutorial", true);
            CorvusGame.Instance.StateManager.PushState(CorvusGame.Instance.SceneManager);
        }

        void arenaModeBtn_Selected(object sender, EventArgs e)
        {
            CorvusGame.Instance.SceneManager.ReloadScenes(false);
            CorvusGame.Instance.SceneManager.ChangeScene("Arena", true);
            CorvusGame.Instance.StateManager.PushState(CorvusGame.Instance.SceneManager);
        }

        void exitBtn_Selected(object sender, EventArgs e)
        {
            CorvusGame.Instance.Exit();
        }

        void testModeBtn_Selected(object sender, EventArgs e)
        {
            CorvusGame.Instance.SceneManager.ReloadScenes(false);
            CorvusGame.Instance.SceneManager.ChangeScene("BasicLevel", true);
            CorvusGame.Instance.StateManager.PushState(CorvusGame.Instance.SceneManager);
        }

    }

}
