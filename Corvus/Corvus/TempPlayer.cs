using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Corvus
{
    class TempPlayer
    {
        Game Game;

        Texture2D texture;
        int textureWidth;
        int textureHeight;
        private GraphicsDeviceManager graphics;

        Rectangle playerRect;
        Vector2 position;

        Keys left;
        Keys right;

        public TempPlayer(Game Game, GraphicsDeviceManager graphics)
        {
            // TODO: Complete member initialization
            this.graphics = graphics;
            this.Game = Game;

            SetupPlayer();
        }

        protected void SetupPlayer()
        {
            texture = Game.Content.Load<Texture2D>("Sprites/Player");
            textureWidth = 20;
            textureHeight = 30;

            this.position = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            playerRect = new Rectangle((int)(this.position.X - textureWidth), (int)(this.position.Y - textureHeight), textureWidth, textureHeight);

            left = Keys.Left;
            right = Keys.Right;
        }

        public void Update(GameTime gameTime)
        {
            UpdatePlayer();

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(right))
            {
                position.X++;
            }

            if (ks.IsKeyDown(left))
            {
                position.X--;
            }
        }

        protected void UpdatePlayer()
        {
            playerRect = new Rectangle((int)(this.position.X - textureWidth), (int)(this.position.Y - textureHeight), textureWidth, textureHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, playerRect, Color.White);
        }
    }
}
