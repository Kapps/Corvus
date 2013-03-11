using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;

namespace Corvus.Components
{
    /// <summary>
    /// A component used to specify the AI for an entity.
    /// </summary>
    public class AIComponent : Component
    {
        //TODO: Not sure if it should be circle, rectangle, or some obscure shape. Right now, it's a rectangle.
        //TODO: Might also make this rectangle based on the direction of the entity. So that the player has a chance to sneak up on enemies.
        //TODO: Create a property that specifies the center of the rectangle. This would also be affected by the direction of the entity.
        /// <summary>
        /// Gets or sets a value that indicates how far this entity will check before reacting. 
        /// The area is centered with the center of the sprite. (Might change later on)
        /// </summary>
        public Vector2 ReactionRange
        {
            get { return _ReactionRange; }
            set { _ReactionRange = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates how much to offset the reaction box with respect to the entities centre. 
        /// </summary>
        public Vector2 Offset
        {
            get { return _OffSet; }
            set { _OffSet = value; }
        }
        
        /// <summary>
        /// Gets or sets a value that indicates the types of entities to search for.
        /// </summary>
        public EntityClassification EntitiesToSearchFor
        {
            get { return _EntitiesToSearchFor; }
            set { _EntitiesToSearchFor = value; }
        }

        private Vector2 _ReactionRange = new Vector2();
        private Vector2 _OffSet = new Vector2();
        private EntityClassification _EntitiesToSearchFor;

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);

            //TODO: Remove this later. I think there is a bug where the entities placed using Tiled are initialized BEFORE
            //      the Scene itself. Basically, OnInitialize() is called on the entity before any SceneSystems are added.
            PhysicsSystem = Parent.Scene.GetSystem<PhysicsSystem>();
            if (PhysicsSystem == null)
                return;

            //TODO: This could be very ineffecient.
            foreach (Entity e in PhysicsSystem.GetEntitiesAtLocation(GetReactionBox()))
            {
                var cc = e.GetComponent<ClassificationComponent>();
                var pc = this.GetDependency<PathComponent>();
                var mc = this.GetDependency<MovementComponent>();

                if (cc.Classification == _EntitiesToSearchFor)
                {
                    //TODO: Make it actually do something. Current does nothing significant.
                    mc.StopWalking();
                    pc.StopFollowing();
                }
                else
                {
                    pc.StartFollowing();
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            MovementComponent = this.GetDependency<MovementComponent>();
            //TODO: Initialize this here. 
            //PhysicsSystem = Parent.Scene.GetSystem<PhysicsSystem>();
        }

        /// <summary>
        /// Gets the reaction box centered on the entity.
        /// </summary>
        private Rectangle GetReactionBox()
        {
            //TODO: Remove this later. This is the same bug as above, i believe.
            if (Camera.Active == null)
                return new Rectangle();

            var center = new Vector2(Parent.Location.Center.X, Parent.Location.Center.Y);

            //There's a bit of a bug when the direction is None. Basically, the Offset.X is gone when that happens.
            var dirSign = CorvusExtensions.GetSign(MovementComponent.CurrentDirection);
            var rectPos = new Vector2((center.X + (dirSign * Offset.X)) - ReactionRange.X / 2, (center.Y + Offset.Y) - ReactionRange.Y / 2);
            Rectangle rect = new Rectangle((int)rectPos.X, (int)rectPos.Y, (int)ReactionRange.X, (int)ReactionRange.Y);
            return rect;
        }

        private MovementComponent MovementComponent;
        private PhysicsSystem PhysicsSystem;
    }
}
