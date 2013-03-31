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
        /// <summary>
        /// Gets or sets a value that indicates how far this entity will check before reacting. 
        /// The area is centered with the center of the sprite.
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

        private DateTime DeathTime
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

        private bool Aggressive
        {
            get { return _Aggressive; }
            set { _Aggressive = value; }
        }

        private Vector2 _ReactionRange = new Vector2();
        private Vector2 _OffSet = new Vector2();
        private EntityClassification _EntitiesToSearchFor;
        private bool _IsReacting = false;
        private bool _AllowMultiJump = true;
        private bool _AIEnabled = true;
        private DateTime _StartOfDeath;
        private bool _DeathStarted;
        private bool _FleeingStarted;
        private bool _Aggressive = true;

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
                    bool projectileFlyingToMe = false;
                    bool entityAttackingMe = false;
                    bool entityAttackable = false;
                    bool entityFollowable = false;
                    Entity entityToFollow = null;

                    //TODO: This could be very ineffecient.
                    //Foreach entity in this entity's reaction box.
                    foreach (Entity e in PhysicsSystem.GetEntitiesAtLocation(GetReactionBox()))
                    {
                        var clc = e.GetComponent<ClassificationComponent>();
                        var coc = e.GetComponent<CombatComponent>();

                        if (clc.Classification == EntityClassification.Player) //If Player
                        {
                            foundEntity = true;

                            entityFollowable = true;
                            entityToFollow = e;

                            if (coc.IsAttacking && EntityWithinAttackRange(e) && EntityFacingMe(e))
                                entityAttackingMe = true;

                            if (!MovementComponent.IsWalking)
                                entityAttackable = true;

                            if (((AttributesComponent.CurrentHealth / AttributesComponent.MaxHealth) * 100) < 25)
                            {
                                FleeingStarted = true;
                            }
                        }
                        else if (clc.Classification == EntityClassification.Projectile) //If Projectile
                        {
                            foundEntity = true;
                            
                            //Basically, projectiles within our rectangle might hit us, so we'll just block.
                            if (EntityGoingToMe(e))
                                projectileFlyingToMe = true;
                        }
                    }

                    if (foundEntity) //If we found an entity.
                    {
                        //We're reacting to an entity.
                        IsReacting = true;

                        //If we're following a path and we're aggressive, stop following the path.
                        if (PathComponent.PathingEnabled && Aggressive)
                        {
                            PathComponent.StopFollowing();
                        }

                        //If we're able to follow an entity and we're aggressive, follow the entity.
                        if (entityFollowable && entityToFollow != null && Aggressive)
                        {
                            FollowEntity(entityToFollow, Time);
                        }

                        //If an entity is attackable and if i (the enemy) am not blocking, attack.
                        if (entityAttackable && !CombatComponent.IsBlocking)
                        {
                            CombatComponent.AttackAI();
                        }

                        //If a projectile is coming to us, or an entity is attacking us, block.
                        if (entityAttackingMe || projectileFlyingToMe)
                        {
                            if (!CombatComponent.IsBlocking)
                                CombatComponent.BeginBlock();
                        }

                        //If there's no projectile coming to us, or no entity attacking us, end the block.
                        if (!entityAttackingMe && !projectileFlyingToMe)
                        {
                            if (CombatComponent.IsBlocking)
                                CombatComponent.EndBlock();
                        }
                    }
                    else //If we found nothing.
                    {
                        IsReacting = false;

                        //Resume following the path.
                        if (!PathComponent.PathingEnabled)
                            PathComponent.StartFollowing();
                    }
                }
                else if (DeathStarted) //DYING AI
                {
                    //Stop blocking if we were doing so at the moment death occured.
                    if (CombatComponent.IsBlocking)
                        CombatComponent.EndBlock();

                    Direction walkDirection = MovementComponent.WalkDirection == Direction.Left ? Direction.Right : Direction.Left;
                    double walkTime = 200;

                    if ((DateTime.Now - DeathTime).TotalMilliseconds > walkTime)
                    {
                        MovementComponent.BeginWalking(walkDirection);
                        DeathTime = DateTime.Now;
                    } 
                }
                else if (FleeingStarted) //FLEEING AI
                {
                    //Stop blocking if we were doing so at the moment fleeing occured.
                    if (CombatComponent.IsBlocking)
                        CombatComponent.EndBlock();

                    Vector2 leftPlatformVector = new Vector2(Parent.Location.Center.X - 50, Parent.Location.Bottom + 1);
                    Vector2 rightPlatformVector = new Vector2(Parent.Location.Center.X + 50, Parent.Location.Bottom + 1);
                    Vector2 leftWallVector = new Vector2(Parent.Location.Center.X - 50, Parent.Location.Center.Y);
                    Vector2 rightWallVector = new Vector2(Parent.Location.Center.X + 50, Parent.Location.Center.Y);
                    bool leftPossible = PhysicsSystem.IsLocationSolid(leftPlatformVector);// && !PhysicsSystem.IsLocationSolid(leftWallVector);
                    bool rightPossible = PhysicsSystem.IsLocationSolid(rightPlatformVector);// && !PhysicsSystem.IsLocationSolid(rightWallVector);

                    if (MovementComponent.CurrentDirection == Direction.Left)
                        if (leftPossible)
                            MovementComponent.BeginWalking(Direction.Left);
                        else
                            MovementComponent.BeginWalking(Direction.Right);
                    else if (MovementComponent.CurrentDirection == Direction.Right)
                        if (rightPossible)
                            MovementComponent.BeginWalking(Direction.Right);
                        else
                            MovementComponent.BeginWalking(Direction.Left);

                    //Begin process of death if entity has run out of health.
                    if (((AttributesComponent.CurrentHealth / AttributesComponent.MaxHealth) * 100) < 10)
                    {
                        if (!DeathStarted)
                            DeathTime = DateTime.Now;

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

                //Ensure they are facing the correct direction, if they move within AI rectangle.
                if (entity.Location.Center.X > e.Location.Center.X)
                {
                    MovementComponent.CurrentDirection = Direction.Left;
                }
                else if (entity.Location.Center.X < e.Location.Center.X)
                {
                    MovementComponent.CurrentDirection = Direction.Right;
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

            if (e.Location.Center.X == Parent.Location.Center.X)
                return true;
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
