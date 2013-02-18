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
		public Vector2 CurrentNode;
		public int ArrivedNode = 5;
		int nodeIndex = 0;

        public PathComponent()
        {
            CurrentNode = new Vector2(250, 768);
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
				float maxWalkVelocity = 10f;

				if(entity.X < CurrentNode.X) {
					entity.VelX = maxWalkVelocity;

					if(mc.CurrDir != Direction.Right) {
						entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("WalkRight");
						mc.CurrDir = Direction.Right;
					}
				} else {
					entity.VelX = maxWalkVelocity * -1;

					if(mc.CurrDir != Direction.Left) {
						entity.GetComponent<SpriteComponent>().Sprite.PlayAnimation("WalkLeft");
						mc.CurrDir = Direction.Left;
					}
				}
			}

			entity.X += entity.VelX;
			entity.Y += entity.VelY;
			base.Update(Time);
		}
	}
}
