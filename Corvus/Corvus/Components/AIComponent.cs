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

        /// <summary>
        /// Gets or sets the bool representing whether or not the AI is reacting to something.
        /// </summary>
        public bool IsReacting
        {
            get { return _IsReacting; }
            set { _IsReacting = value; }
        }

        /// <summary>
        /// Gets or sets the bool respresenting whether or not the AI is following something.
        /// </summary>
        public bool IsFollowingEntity
        {
            get { return _IsFollowingEntity; }
            set { _IsFollowingEntity = value; }
        }

        /// <summary>
        /// Gets or sets the bool respresenting whether or not the AI is enabled.
        /// </summary>
        public bool AIEnabled
        {
            get { return _AIEnabled; }
            set { _AIEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow this entity to jump multiple times.
        /// </summary>
        public bool AllowMultiJump
        {
            get { return _AllowMultiJump; }
            set { _AllowMultiJump = value; }
        }

        private DateTime StartOfDeath
        {
            get { return _StartOfDeath; }
            set { _StartOfDeath = value; }
        }

        private bool DeathStarted
        {
            get { return _DeathStarted; }
            set { _DeathStarted = value; }
        }

        private bool FleeingStarted
        {
            get { return _FleeingStarted; }
            set { _FleeingStarted = value; }
        }

        private Entity FleeingFromEntity
        {
            get { return _FleeingFromEntity; }
            set { _FleeingFromEntity = value; }
        }

        private Vector2 _ReactionRange = new Vector2();
        private Vector2 _OffSet = new Vector2();
        private EntityClassification _EntitiesToSearchFor;
        private bool _IsReacting = false;
        private bool _AllowMultiJump = true;
        private bool _IsFollowingEntity = false;
        private bool _AIEnabled = true;
        private DateTime _StartOfDeath;
        private bool _DeathStarted;
        private bool _FleeingStarted;
        private Entity _FleeingFromEntity;

        private PhysicsSystem PhysicsSystem;
        private MovementComponent MovementComponent;
        private PathComponent PathComponent;
        private CombatComponent CombatComponent;
        private PhysicsComponent PhysicsComponent;
        private AttributesComponent AttributesComponent;
        private SpriteComponent SpriteComponent;
        private HealthBarComponent HealthBarComponent;
        private DamageComponent DamageComponent;

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);

            if (AIEnabled)
            {
                if (!DeathStarted && !FleeingStarted) //NORMAL AI
                {
                    bool foundEntity = false;

                    //TODO: This could be very ineffecient.
                    //Foreach entity in this entity's reaction box.
                    foreach (Entity e in PhysicsSystem.GetEntitiesAtLocation(GetReactionBox()))
                    {
                        var clc = e.GetComponent<ClassificationComponent>();
                        var coc = e.GetComponent<CombatComponent>();

                        if (clc.Classification == EntityClassification.Player) //If Player
                        {
                            foundEntity = true;
                            IsFollowingEntity = true;
                            IsReacting = true;

                            //If we're following a path, stop.
                            if (PathComponent.PathingEnabled)
                                PathComponent.StopFollowing();

                            //Follow the entity.
                            FollowEntity(e, Time);

                            Random r = new Random();
                            int blockChance = r.Next(1, 5);
                            //If the entity is attacking and is within attacking range, and is facing us, and we're not currently blocking, and blockChance is good, block.
                            if (coc.IsAttackingMelee && EntityWithinAttackRange(e) && EntityFacingMe(e) && !coc.IsBlocking && blockChance >= 3)
                                CombatComponent.BeginBlock();

                            //If the entity isn't attacking, stop blocking.
                            if (!coc.IsAttackingMelee && CombatComponent.IsBlocking)
                                CombatComponent.EndBlock();

                            if (!MovementComponent.IsWalking)
                                CombatComponent.AttackAI();

                            if (((AttributesComponent.CurrentHealth / AttributesComponent.MaxHealth) * 100) < 25)
                            {
                                FleeingStarted = true;
                                FleeingFromEntity = e;
                            }
                        }
                        else if (clc.Classification == EntityClassification.Projectile) //If Projectile
                        {
                            foundEntity = true;
                            IsFollowingEntity = false;
                            IsReacting = true;

                            //If we're following a path, stop.
                            if (PathComponent.PathingEnabled)
                                PathComponent.StopFollowing();

                            //Basically, projectiles within our rectangle might hit us, so we'll just block.
                            if (EntityGoingToMe(e))
                                CombatComponent.BeginBlock();
                            else
                                if (CombatComponent.IsBlocking)
                                    CombatComponent.EndBlock();
                        }
                    }

                    //If no entities were found, resume normal pathing and end blocking.
                    if (!foundEntity)
                    {
                        IsReacting = false;
                        IsFollowingEntity = false;

                        if (!PathComponent.PathingEnabled)
                            PathComponent.StartFollowing();

                        if (CombatComponent.IsBlocking)
                            CombatComponent.EndBlock();
                    }
                }
                else if (DeathStarted) //DYING AI
                {
                    double totalMs = (DateTime.Now - StartOfDeath).TotalMilliseconds;
                    double walkTime = 250;

                    if (totalMs < walkTime)
                        MovementComponent.BeginWalking(Direction.Left);
                    else if (totalMs >= walkTime && totalMs < walkTime*2)
                        MovementComponent.BeginWalking(Direction.Right);
                    else if (totalMs >= walkTime*2 && totalMs < walkTime*3)
                        MovementComponent.BeginWalking(Direction.Left);
                    else if (totalMs >= walkTime*3 && totalMs < walkTime*4)
                        MovementComponent.BeginWalking(Direction.Right);
                    else
                        Parent.Dispose();
                }
                else if (FleeingStarted) //FLEEING AI
                {
                    Direction entityDir = GetEntityDirection(FleeingFromEntity);

                    if (entityDir == Direction.Left)
                        MovementComponent.BeginWalking(Direction.Right);
                    else
                        MovementComponent.BeginWalking(Direction.Left);

                    //Begin process of death if entity has run out of health.
                    if (AttributesComponent.CurrentHealth <= 0)
                    {
                        if (!DeathStarted)
                            StartOfDeath = DateTime.Now;

                        DeathStarted = true;
                    }
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            MovementComponent = this.GetDependency<MovementComponent>();
            PathComponent = this.GetDependency<PathComponent>();
            MovementComponent = this.GetDependency<MovementComponent>();
            CombatComponent = this.GetDependency<CombatComponent>();
            PhysicsSystem = Parent.Scene.GetSystem<PhysicsSystem>();
            PhysicsComponent = this.GetDependency<PhysicsComponent>();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            SpriteComponent = this.GetDependency<SpriteComponent>();
            HealthBarComponent = this.GetDependency<HealthBarComponent>();
            DamageComponent = this.GetDependency<DamageComponent>();
        }

        /// <summary>
        /// Gets the reaction box centered on the entity.
        /// </summary>
        private Rectangle GetReactionBox()
        {
            //TODO: Remove this later. Some weird bug that i can't explain.
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
            var AllowMultiJump = false;
            Random r = new Random();
            float attackRange = AttributesComponent.MeleeAttackRange.X;

            if (entity.Location.Contains((int)e.Location.Center.X, (int)e.Location.Center.Y))
            {
                if (!PhysicsComponent.IsGrounded)
                    return; // Do nothing, just wait for us to fall on our location.
            }
            else
            {
                bool MissingHorizontally = e.Location.Center.X - attackRange > entity.Location.Right || e.Location.Center.X + attackRange < entity.Location.Left;
                if (entity.Location.Bottom > e.Location.Bottom && !MissingHorizontally)
                {
                    MovementComponent.Jump(AllowMultiJump);
                }
                if (MissingHorizontally)
                {
                    if (entity.Location.Center.X > e.Location.Center.X + attackRange)
                    {
                        MovementComponent.BeginWalking(Direction.Left);
                    }
                    else if (entity.Location.Center.X < e.Location.Center.X - attackRange)
                    {
                        MovementComponent.BeginWalking(Direction.Right);
                    }
                }
                else
                {
                    if (MovementComponent.IsWalking)
                        MovementComponent.StopWalking(); 
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

        private bool EntityGoingToMe(Entity e)
        {
            var pc = e.GetComponent<PhysicsComponent>();

            if (e.Location.Center.X == Parent.Location.Center.X) //Won't get stuck inside entities now. Didn't before, but slight chance that it could happen. Besides, it's already hit you at this point.
                return false;
            else if ((e.Location.Center.X < Parent.Location.Center.X) && pc.VelocityX > 0)
                return true;
            else if ((e.Location.Center.X > Parent.Location.Center.X) && pc.VelocityX < 0)
                return true;
            else
                return false;
        }

        private bool EntityWithinAttackRange(Entity e)
        {
            var atc = e.GetComponent<AttributesComponent>();

            //TODO: Figure out why the attackrange isn't quite correct... I mean, this'll (+10) likely fix it in almost every situation, so not priority, but still.
            if (Math.Abs(Parent.Location.Center.X - e.Location.Center.X) <= atc.MeleeAttackRange.X + 10)
                return true;
            else
                return false;
        }

        private Direction GetEntityDirection(Entity e)
        {
            if (e.Location.Center.X < Parent.Location.Center.X)
                return Direction.Left;
            else if (e.Location.Center.X > Parent.Location.Center.X)
                return Direction.Right;
            else
                return Direction.Left;
        }

        /// <summary>
        /// Stops AI and ends any current AI actions, such as walking or blocking.
        /// </summary>
        public void StopAI()
        {
            AIEnabled = false;

            IsReacting = false;
            IsFollowingEntity = false;

            if (CombatComponent.IsBlocking)
                CombatComponent.EndBlock();

            if (MovementComponent.IsWalking)
                MovementComponent.StopWalking();
        }

        /// <summary>
        /// Resumes AI.
        /// </summary>
        public void StartAI()
        {
            AIEnabled = true;
        }
    }
}
