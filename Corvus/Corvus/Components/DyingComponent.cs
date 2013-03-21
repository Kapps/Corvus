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
    /// A component to manage a dying entity. Basically just draws the dying animation. Should only be included in DyingEntity.txt.
    /// </summary>
    public class DyingComponent : Component
    {
        private float _Duration = 0f;
        private SpriteComponent SC;
        private TimeSpan _Timer = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the duration this entity will play for.
        /// </summary>
        public float Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SC = this.GetDependency<SpriteComponent>();
        }

        protected override void OnUpdate(GameTime Time)
        {
            base.OnUpdate(Time);

            _Timer += Time.ElapsedGameTime;
            if (_Timer >= TimeSpan.FromSeconds(Duration))
                Parent.Dispose();
        }

        /// <summary>
        /// Creates a dying entity.
        /// </summary>
        /// <param name="entity">The entity to replace.</param>
        /// <param name="spriteName">The name of the dying animation.</param>
        /// <param name="duration">How long this animation should last.</param>
        public static void CreateDyingEntity(Entity entity, string spriteName, float duration)
        {
            var deaded = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint("DyingEntity").CreateEntity();
            deaded.Size = entity.Size;
            deaded.Position = entity.Position;
            entity.Scene.AddEntity(deaded);

            var sc = deaded.GetComponent<SpriteComponent>();
            sc.Sprite = CorvusGame.Instance.GlobalContent.LoadSprite(spriteName);
            var anim = sc.Sprite.ActiveAnimation;
            sc.Sprite.PlayAnimation(anim.Name, TimeSpan.FromSeconds(duration)); //TODO: Potential issues involved with this.
            var dc = deaded.GetComponent<DyingComponent>();
            dc.Duration = duration;

            var FTC = entity.GetComponent<FloatingTextComponent>();
            if (FTC != null)
            {
                var deadedFtc = deaded.GetComponent<FloatingTextComponent>();
                deadedFtc.FloatingTextList = FTC.FloatingTextList;
            }
        }

    }
}
