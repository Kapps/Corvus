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
    class Path
    {
        //Lazy implementation since not sure if permanent yet.

        public List<Vector2> Nodes = new List<Vector2>();
        public Vector2 CurrentNode;
        public int ArrivedNode = 5;
        int nodeIndex = 0;

        public void AddNode(Vector2 node)
        {
            if (Nodes.Count() == 0)
            {
                CurrentNode = node;
                nodeIndex = 0;
            }

            Nodes.Add(node);
        }

        public void NextNode()
        {
            if (CurrentNode == Nodes.Last())
            {
                CurrentNode = Nodes.First();
                nodeIndex = 0;
            }
            else
            {
                CurrentNode = Nodes[nodeIndex+1];
            }
        }
    }
}
