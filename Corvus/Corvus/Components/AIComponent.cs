using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;
using CorvEngine;

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

        public bool IsReacting
        {
            get { return _IsReacting; }
            set { _IsReacting = value; }
        }

        public bool IsFollowingEntity
        {
            get { return _IsFollowingEntity; }
            set { _IsFollowingEntity = value; }
        }

        public DateTime LastJump
        {
            get { return _LastJump; }
            set { _LastJump = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow this entity to jump multiple times.
        /// </summary>
        public bool AllowMultiJump
        {
            get { return _AllowMultiJump; }
            set { _AllowMultiJump = value; }
        }

        private Vector2 _ReactionRange = new Vector2();
        private Vector2 _OffSet = new Vector2();
        private EntityClassification _EntitiesToSearchFor;
        private bool _IsReacting = false;
        private DateTime _LastJump = DateTime.Now;
        private bool _AllowMultiJump = true;
        private bool _IsFollowingEntity = false;

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);

            //TODO: Remove this later. I think there is a bug where the entities placed using Tiled are initialized BEFORE
            //      the Scene itself. Basically, OnInitialize() is called on the entity before any SceneSystems are added.
            PhysicsSystem = Parent.Scene.GetSystem<PhysicsSystem>();
            if (PhysicsSystem == null)
                return;

            var pc = this.GetDependency<PathComponent>();
            var mc = this.GetDependency<MovementComponent>();
            var cc = this.GetDependency<CombatComponent>();

            bool foundEntity = false;
            bool foundPlayer = false;
            bool foundProjectile = false;

            //TODO: This could be very ineffecient.
            //Foreach entity in this entity's reaction box.
            foreach (Entity e in PhysicsSystem.GetEntitiesAtLocation(GetReactionBox()))
            {
                var clc = e.GetComponent<ClassificationComponent>();
                var coc = e.GetComponent<CombatComponent>();
                

                if (clc.Classification == EntityClassification.Player) //If Player
                {
                    foundEntity = true;
                    foundPlayer = true;
                    IsFollowingEntity = true;
                    IsReacting = true;

                    //If we're following a path, stop.
                    if (pc.IsPathing)
                        pc.StopFollowing();

                    //Follow the player's entity.
                    FollowEntity(e, Time); 

                    //If the player is attacking and is within attacking range, block.
                    if (coc.IsAttacking && EntityWithinAttackRange(e) && EntityFacingMe(e))
                        cc.BeginBlock();

                    //If the player isn't attacking, stop blocking.
                    if (!coc.IsAttacking && cc.IsBlocking)
                        cc.EndBlock();
                }
                else if (clc.Classification == EntityClassification.Projectile) //If Projectile
                {
                    foundEntity = true;
                    foundProjectile = true;
                    IsFollowingEntity = false;
                    IsReacting = true;

                    //If we're following a path, stop.
                    if (pc.IsPathing)
                        pc.StopFollowing();

                    //Basically, projectiles within our rectangle might hit us, so we'll just block.
                    if (EntityFlyingToMe(e))
                        cc.BeginBlock();
                    else
                        cc.EndBlock();
                }
            }

            //If no entities were found, resume normal pathing and end blocking.
            if (!foundEntity)
            {
                IsReacting = false;
                IsFollowingEntity = false;

                if (!pc.IsPathing)
                    pc.StartFollowing();

                if (cc.IsBlocking)
                    cc.EndBlock();
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

        //TODO: Make this account for attack range and entity size.
        //TODO: Maybe make this not allow them to leave their platform as well, just to avoid issues with pathing.
        private void FollowEntity(Entity e, GameTime Time)
        {
            var entity = this.Parent;
            var mc = entity.GetComponent<MovementComponent>();
            var pc = entity.GetComponent<PhysicsComponent>();
            var ps = Scene.GetSystem<PhysicsSystem>();
            var cc = entity.GetComponent<CombatComponent>();
            var AllowMultiJump = false;
            int followDistance = 50;

            if (entity.Location.Contains((int)e.Location.Center.X, (int)e.Location.Center.Y))
            {
                if (!pc.IsGrounded)
                    return; // Do nothing, just wait for us to fall on our location.
            }
            else
            {
                bool MissingHorizontally = e.Location.Center.X - followDistance > entity.Location.Right || e.Location.Center.X + followDistance < entity.Location.Left;
                if (entity.Location.Bottom > e.Location.Bottom && !MissingHorizontally)
                {
                    mc.Jump(AllowMultiJump);
                    LastJump = DateTime.Now;
                }
                if (MissingHorizontally)
                {
                    if (entity.Location.Center.X > e.Location.Center.X + followDistance)
                    {
                        mc.BeginWalking(Direction.Left);
                    }
                    else if (entity.Location.Center.X < e.Location.Center.X - followDistance)
                    {
                        mc.BeginWalking(Direction.Right);
                    }
                }
                else
                {
                    //cc.AttackMelee();
                    mc.StopWalking(); //this is pointless.
                }
            }
        }

        private bool EntityFacingMe(Entity e)
        {
            var mc = e.GetComponent<MovementComponent>();
            
            if (e.Location.Center.X == Parent.Location.Center.X)
                return true;
            else if ((e.Location.Center.X < Parent.Location.Center.X) && mc.CurrentDirection == Direction.Right)
                return true;
            else if ((e.Location.Center.X > Parent.Location.Center.X) && mc.CurrentDirection == Direction.Left)
                return true;
            else
                return false;
        }

        private bool EntityFlyingToMe(Entity e)
        {
            var pc = e.GetComponent<PhysicsComponent>();

            if (e.Location.Center.X == Parent.Location.Center.X)
                return true;
            else if ((e.Location.Center.X < Parent.Location.Center.X) && pc.VelocityX >= 0)
                return true;
            else if ((e.Location.Center.X > Parent.Location.Center.X) && pc.VelocityX <= 0)
                return true;
            else
                return false;
        }

        private bool EntityWithinAttackRange(Entity e)
        {
            var atc = e.GetComponent<AttributesComponent>();

            if (Math.Abs(Parent.Location.Center.X - e.Location.Center.X) < atc.MeleeAttackRange.X)
                return true;
            else
                return false;
        }

        private MovementComponent MovementComponent;
        private PhysicsSystem PhysicsSystem;
    }
}
