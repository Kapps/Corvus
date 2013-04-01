using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine;
using CorvEngine.Graphics;
using CorvEngine.Components;


namespace Corvus.Components.Gameplay
{
    public class ShieldAnimation
    {
        private Texture2D _Effect;
        private Entity _Entity;
        private bool _ShowEffect = false;
        private MovementComponent MC;

        public bool ShowEffect
        {
            get { return _ShowEffect; }
            set { _ShowEffect = value; }
        }

        public ShieldAnimation(Entity entity, string texture)
        {
            _Entity = entity;
            _Effect = CorvusGame.Instance.GlobalContent.Load<Texture2D>(texture);
            MC = _Entity.GetComponent<MovementComponent>();
        }

        public void Update(GameTime time)
        {

        }

        public void Draw()
        {
            if (!ShowEffect)
                return;
            float xpos = (MC.CurrentDirection == Direction.Left) ? _Entity.Location.Left -1 : _Entity.Location.Right + 1;
            var pos = new Vector2(xpos , _Entity.Location.Top);
            var toscreen = Camera.Active.WorldToScreen(pos);
            var rect = new Rectangle((int)toscreen.X, (int)toscreen.Y, 3, (int)_Entity.Size.Y);
            CorvusGame.Instance.SpriteBatch.Draw(_Effect, rect, _Effect.Bounds, Color.Gray);
        }


    }
}
