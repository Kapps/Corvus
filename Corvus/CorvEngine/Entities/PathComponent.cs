using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities
{
	class PathComponent : Component {
		//Lazy implementation since not sure if permanent yet.

		public List<Vector2> Nodes { get; private set; }
		public Vector2 CurrentNode { get; private set; }
		public int ArrivedNode = 5;
		int nodeIndex = 0;

        public PathComponent()
        {
            
        }

        public void AddNode(Vector2 node)
        {
            if (Nodes.Count() == 0)
            {
                CurrentNode = node;
                nodeIndex = 0;
            }
			Nodes.Add(node);
		}

		public void NextNode() {
			if(CurrentNode == Nodes.Last()) {
				CurrentNode = Nodes.First();
				nodeIndex = 0;
			} else {
				CurrentNode = Nodes[nodeIndex + 1];
			}
		}

		public override void Update(GameTime Time) {
			Entity entity = this.Parent;
			MovementComponent mc = entity.GetComponent<MovementComponent>();

			if(Vector2.Distance(entity.Position, CurrentNode) < ArrivedNode) {
				NextNode();
			} else {
				if(entity.X < CurrentNode.X) {
					mc.Walk(Direction.Right);
                    mc.ApplyPhysics(Time, entity.Scene);
				} else {
					mc.Walk(Direction.Left);
                    mc.ApplyPhysics(Time, entity.Scene);
				}
			}

			//entity.X += entity.VelX * Time.GetTimeScalar();
			//entity.Y += entity.VelY * Time.GetTimeScalar();
			base.Update(Time);
		}
	}
}
