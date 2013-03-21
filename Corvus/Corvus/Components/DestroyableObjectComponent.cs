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
        private string _DyingSprite = "";
        private float _DyingDuration = 0f;
        private int _AwardedExperience = 0;
        private bool _DropsItem = false;
        private List<string> _DroppableItems = new List<string>();
        private float _DropRate = 0;
        private float _DropCoefficient = 2;

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

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AC = this.GetDependency<AttributesComponent>();
            if(!AC.IsDiedRegistered)
                AC.Died += AC_Died;
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

            //Give exp.
            //CorvBase.Instance.Players
            foreach (var p in CorvBase.Instance.Players)
            {
                var pcp = p.Character.GetComponent<PlayerControlComponent>();
                pcp.CurrentExperience += AwardedExperience;
            }
            
            DyingComponent.CreateDyingEntity(this.Parent, DyingSprite, DyingDuration);
            obj.Parent.Dispose();
        }
    }
}
