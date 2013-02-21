using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Components
{
	class PathComponent : Component {
		//Lazy implementation since not sure if permanent yet.

		public List<Vector2> Nodes { get; private set; }
		public Vector2 CurrentNode { get; private set; }
		public int ArrivedNode = 25;
		int nodeIndex = 0;

        public PathComponent()
        {
            
        }

        /// <summary>
        /// Add a new node to the list of nodes.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Vector2 node)
        {
            if (Nodes.Count() == 0)
            {
                CurrentNode = node;
                nodeIndex = 0;
            }
			Nodes.Add(node);
		}

        /// <summary>
        /// Sets the current node to the next node, and if it's last one, it loops over to the start.
        /// </summary>
		public void NextNode() {
			if(CurrentNode == Nodes.Last()) {
				CurrentNode = Nodes.First();
				nodeIndex = 0;
			} else {
				CurrentNode = Nodes[nodeIndex + 1];
			}
		}

        /// <summary>
        /// Decides which direction the AI should walk to follow the path. Simple x value check.
        /// </summary>
        /// <param name="Time"></param>
		protected override void OnUpdate(GameTime Time) {
			Entity entity = this.Parent;
			MovementComponent mc = entity.GetComponent<MovementComponent>();

            //Formerly Vector2.Distance(entity.Position, CurrentNode) for Y stuff, but not needed.
			if(Math.Abs(entity.Position.X - CurrentNode.X) < ArrivedNode) {
				NextNode();
			} else {
				if(entity.X < CurrentNode.X) {
					mc.Walk(Direction.Right);
				} else {
					mc.Walk(Direction.Left);
				}
			}

			//entity.X += entity.VelX * Time.GetTimeScalar();
			//entity.Y += entity.VelY * Time.GetTimeScalar();
			base.OnUpdate(Time);
		}
	}
}
