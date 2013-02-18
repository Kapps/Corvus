using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorvEngine;
using CorvEngine.Entities;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Corvus
{
    class TempEnemy
    {
        Entity entity;
        Path path = new Path();
        float maxWalkVelocity = 5f;
        enum Direction
        {
            None,
            Down,
            Left,
            Right,
            Up
        }

        Direction CurrDir = Direction.Down;

        public TempEnemy()
        {
            path.AddNode(new Vector2(250, Camera.Active.Viewport.Height));
            path.AddNode(new Vector2(750, Camera.Active.Viewport.Height));
            SetupEnemy();
        }

        protected void SetupEnemy()
        {
            BlueprintParser.ParseBlueprint(File.ReadAllText("TestEntityEnemy.txt"));
            var Blueprint = EntityBlueprint.GetBlueprint("TestEntityEnemy");
            entity = Blueprint.CreateEntity();
            // This stuff is obviously things that the ctor should handle.
            // And things like size should probably be dependent upon the actual animation being played.
            entity.Size = new Vector2(48, 32);
            entity.Position = new Vector2(entity.Location.Width + 100, Camera.Active.Viewport.Height);
            entity.Velocity = new Vector2(0, 0);
            entity.Initialize(null);
        }

        public void Update(GameTime gameTime)
        {
            if (Vector2.Distance(entity.Position, path.CurrentNode) < path.ArrivedNode)
            {
                path.NextNode();
            }
            else
            {
                if (entity.X < path.CurrentNode.X)
                {
                    entity.VelX = maxWalkVelocity;

                    if (CurrDir != Direction.Right)
                    {
                        entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("WalkRight");
                        CurrDir = Direction.Right;
                    }
                }
                else
                {
                    entity.VelX = maxWalkVelocity * -1;

                    if (CurrDir != Direction.Left)
                    {
                        entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("WalkLeft");
                        CurrDir = Direction.Left;
                    }
                }
            }

            entity.X += entity.VelX;
            entity.Y += entity.VelY;

            entity.Update(gameTime);
        }

        public void Draw()
        {
            entity.Draw();
        }
    }
}
