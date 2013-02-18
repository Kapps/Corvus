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

        public TempEnemy()
        {
            SetupEnemy();
        }

        protected void SetupEnemy()
        {
            BlueprintParser.ParseBlueprint(File.ReadAllText("Entities/TestEntityEnemy.txt"));
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
            entity.Update(gameTime);
        }

        public void Draw()
        {
            entity.Draw();
        }
    }
}
