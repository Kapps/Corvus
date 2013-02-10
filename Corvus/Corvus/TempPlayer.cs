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

        //Player texture
        Texture2D texture;
        int textureWidth = 32;
        int textureHeight = 48;
        private GraphicsDeviceManager graphics;

        //Player hitboxes and position.
        Rectangle playerRect;
        Vector2 position;

        //Controls
        Keys left = Keys.Left;
        Keys right = Keys.Right;

        //Animation
        bool isWalkingLeft = false;
        bool isWalkingRight = false;
        int frameCount=0; //Current frame position.
        Point frameSize = new Point(32, 48);
        Point frameWalkingLeftStart = new Point(0, 48);
        Point frameWalkingRightStart = new Point(0, 96);
        Rectangle spriteArea; //Which area of the sprite to draw.

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

            this.position = new Vector2(0 + (textureWidth), graphics.GraphicsDevice.Viewport.Height);
            playerRect = new Rectangle((int)(this.position.X - textureWidth), (int)(this.position.Y - textureHeight), textureWidth, textureHeight);
        }

        public void Update(GameTime gameTime)
        {
            UpdatePlayer();

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(left))
            {
                position.X-=4;
                frameCount++;

                isWalkingLeft = true;
                isWalkingRight = false;
            }
            else if (ks.IsKeyDown(right))
            {
                position.X+=4;
                frameCount++;

                isWalkingLeft = false;
                isWalkingRight = true;
            }
            else
            {
                frameCount = 0; //Reset frame count when player stops moving.
            }

            if (frameCount >= 4)
                frameCount = 0;
        }

        protected void UpdatePlayer()
        {
            playerRect = new Rectangle((int)(this.position.X - textureWidth), (int)(this.position.Y - textureHeight), textureWidth, textureHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int x = 0;
            int y = 96;

            if (isWalkingLeft)
            {
                x = frameWalkingLeftStart.X + (frameCount * frameSize.X);
                y = frameWalkingLeftStart.Y;

            }
            else if (isWalkingRight)
            {
                x = frameWalkingRightStart.X + (frameCount * frameSize.X);
                y = frameWalkingRightStart.Y;
            }

            spriteArea = new Rectangle(x, y, frameSize.X, frameSize.Y);

            spriteBatch.Draw(texture, playerRect, spriteArea , Color.White);
        }
    }
}
