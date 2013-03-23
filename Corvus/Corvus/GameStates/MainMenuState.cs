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
    public class MainMenuState : GameState
    {
        private ControlManagerComponent _ControlManager;

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
            //this is all testing.
            SpriteFont font = CorvBase.Instance.GlobalContent.Load<SpriteFont>("Fonts/testfont");
            Label lbl1 = new Label(font);
            lbl1.Text = "Hello world";
            lbl1.Position = new Vector2(200f);
            lbl1.Scale = 2.5f;
            _ControlManager.AddControl(lbl1);

            Image img = new Image(CorvBase.Instance.GlobalContent.Load<Texture2D>("Sprites/Effects/Explosion1"));
            img.Position = new Vector2(500f);
            _ControlManager.AddControl(img);

            LinkButton btn = new LinkButton(font);
            btn.Text = "New Game";
            btn.Position = new Vector2(0f);
            btn.Selected += (sender, e) =>
            {
                CorvusGame.Instance.SceneManager.ChangeScene("BasicLevel");
                CorvBase.Instance.StateManager.PushState(CorvusGame.Instance.SceneManager);
            };
            _ControlManager.AddControl(btn);

            LinkButton btn1 = new LinkButton(font);
            btn1.Text = "Button Two";
            btn1.Position = new Vector2(0f, 30f);
            _ControlManager.AddControl(btn1);

            LinkButton btn2 = new LinkButton(font);
            btn2.Text = "Button Three";
            btn2.Position = new Vector2(0f, 60f);
            _ControlManager.AddControl(btn2);


            _ControlManager.SetFocus();
            this.AddComponent(_ControlManager);

            //TODO: Not sure how to do binds for controls. Temporary for now.
            
        }

    }

}
