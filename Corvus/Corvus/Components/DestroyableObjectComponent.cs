using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorvEngine.Components;
using CorvEngine;
using CorvEngine.Components.Blueprints;

namespace Corvus.Components
{
    /// <summary>
    /// A component to handle entity death. Can drop items or weapons.
    /// Player should not have this.
    /// </summary>
    public class DestroyableObjectComponent : Component
    {
        private AttributesComponent AC;
        private ArenaSystem ArenaSystem;
        private string _DyingSprite = "";
        private float _DyingDuration = 0f;
        private string _DyingSound = "";
        private int _AwardedExperience = 0;
        private bool _DropsItem = false;
        private List<string> _DroppableItems = new List<string>();
        private float _DropRate = 0;
        private float _DropCoefficient = 2;
        private int _CoinsValue = 0;
        
        /// <summary>
        /// Gets or sets the dying sprite.
        /// </summary>
        public string DyingSprite
        {
            get { return _DyingSprite; }
            set { _DyingSprite = value; }
        }

        /// <summary>
        /// Gets or sets the dying duration.
        /// </summary>
        public float DyingDuration
        {
            get { return _DyingDuration; }
            set { _DyingDuration = Math.Max(value, 0f); }
        }

        /// <summary>
        /// Gets or sets the sound effect to play when this entity dies.
        /// </summary>
        public string DyingSound
        {
            get { return _DyingSound; }
            set { _DyingSound = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates how much experience is awarded when this entity dies.
        /// </summary>
        public int AwardedExperience
        {
            get { return _AwardedExperience; }
            set { _AwardedExperience = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that this entity drops an item on death.
        /// </summary>
        public bool DropsItem
        {
            get { return _DropsItem; }
            set { _DropsItem = value; }
        }

        /// <summary>
        /// Gets or sets the items to drop.
        /// </summary>
        public List<string> DroppableItems
        {
            get { return _DroppableItems; }
            set { _DroppableItems = value; }
        }
        
        /// <summary>
        /// Gets or sets the drop rate.
        /// </summary>
        public float DropRate
        {
            get { return _DropRate; }
            set { _DropRate = MathHelper.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets or sets the drop coefficient.
        /// </summary>
        public float DropCoefficient
        {
            get { return _DropCoefficient; }
            set { _DropCoefficient = Math.Max(value, 1f); }
        }

        /// <summary>
        /// Gets or sets the number of coins this entity drops.
        /// </summary>
        public int CoinsValue
        {
            get { return _CoinsValue; }
            set { _CoinsValue = value; }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AC = this.GetDependency<AttributesComponent>();
            if(!AC.IsDiedRegistered)
                AC.Died += AC_Died;
            ArenaSystem = Scene.GetSystem<ArenaSystem>();
        }
        
        void AC_Died(AttributesComponent obj)
        {
            //Drops item
            if (DropsItem && DroppableItems.Count != 0)
            {
                Random rand = new Random();
                float chance = (float)rand.NextDouble();
                float droprate = DropRate;
                float floor = 0f;
                string item = "";
                foreach (string i in DroppableItems)
                {
                    if (floor < chance && chance <=(droprate + floor))
                    {
                        item = i;
                        break;
                    }
                    floor += droprate;
                    droprate /= DropCoefficient;
                }
                if (!string.IsNullOrEmpty(item))
                {
                    var powerup = EntityBlueprint.GetBlueprint(item).CreateEntity();
                    powerup.Size = new Vector2(16, 16);
                    powerup.Position = new Vector2(Parent.Location.Center.X, Parent.Location.Top);
                    Parent.Scene.AddEntity(powerup);
                }
            }

            //Drop coins
            int c = CoinsValue;
            int gold = (c < 5) ? 0 : c /= 5 ;
            int silver = (c < 3) ? 0 : c /= 3;
            int bronze = c;

            for (int i = 0; i < gold; i++)
                GenerateCoinEntity("Coin_Gold", 2f - ((float)i) / 10);
            for (int i = 0; i < silver; i++)
                GenerateCoinEntity("Coin_Silver", 1.5f - ((float)i) / 10);
            for (int i = 0; i < bronze; i++)
                GenerateCoinEntity("Coin_Bronze", 1f - ((float)i) / 10);
            AudioManager.PlaySoundEffect("CoinDrop");
            //Give exp.
            //CorvBase.Instance.Players
            foreach (var p in CorvBase.Instance.Players)
            {
                var pcp = p.Character.GetComponent<PlayerControlComponent>();
                pcp.CurrentExperience += AwardedExperience;
            }

            //play sound
            AudioManager.PlaySoundEffect(DyingSound);
            
            DyingComponent.CreateDyingEntity(this.Parent, DyingSprite, DyingDuration);
            obj.Parent.Dispose();

            //Maybe not the best place for this, but definitely the easiest for now.
            //Adding it to ArenaSystem would require further tracking of entities tabulated.
            //if (ArenaSystem != null)
            //{
            //    ArenaSystem.TotalEntitiesKilled++;
            //    //ArenaSystem.TotalEntitiesKilledWave++;
            //}
        }

        private void GenerateCoinEntity(string coinName, float xmod)
        {
            var c = EntityBlueprint.GetBlueprint(coinName).CreateEntity();
            c.Size = new Vector2(12, 12);
            c.Position = new Vector2(Parent.Location.Center.X, Parent.Location.Top);
            Parent.Scene.AddEntity(c);
            
            Random rand = new Random();
            var pc = c.GetComponent<PhysicsComponent>();
            pc.HorizontalDragCoefficient = 0.01f;
            pc.GravityCoefficient = 0.2f;
            var mc = this.GetDependency<MovementComponent>();
            pc.VelocityX = ((mc == null) ? ((rand.Next(0, 5) <= 2) ? 1 : -1) : -CorvusExtensions.GetSign(mc.CurrentDirection)) * (50f * xmod);
        }
    }
}
