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
    public class OptionsState : GameState
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

        public OptionsState()
            : base()
        {
            _ControlManager = new ControlManagerComponent(this);
            SpriteFont font = CorvusGame.Instance.GlobalContent.Load<SpriteFont>("Fonts/MainMenuFont");


            Image bg = new Image(CorvusGame.Instance.GlobalContent.Load<Texture2D>("Interface/PausedBackgroundalt"));
            bg.Size = new Vector2(UIHelper.Viewport.Width, UIHelper.Viewport.Height);
            _ControlManager.AddControl(bg);

            LinkButton backBtn = new LinkButton(font);
            backBtn.Text = "Back";
            backBtn.Selected += backBtn_Selected;
            backBtn.Position = UIHelper.AlignControl(backBtn.Size, HorizontalAlignment.Center, VerticalAlignment.Center) + new Vector2(0f, 50f);
            _ControlManager.AddControl(backBtn);

            _ControlManager.SetFocus();
            this.AddComponent(_ControlManager);
        }


        void backBtn_Selected(object sender, EventArgs e)
        {

            //Hacky method. 
            CorvusGame.Instance.StateManager.PopState();
            CorvusGame.Instance.StateManager.PopState();
        }

    }
}
