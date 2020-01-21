using System;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using Ninject;
using System.Collections.Generic;

namespace Puffin.Core.Ecs
{
    /// <summary>
    /// A component for overlapping. Two entities with OverlapComponents will fire events when they overlap each other.
    /// </summary>
    public class OverlapComponent : Component
    {
        /// <summary>
        /// The size of area that we check for overlaps. This can be bigger or smaller than the sprite,
        /// or maybe the entity has no sprite at all.
        /// </summary>
        internal readonly Tuple<int, int> Size = new Tuple<int, int>(0, 0);

        /// <summary>
        /// The offset of this overlap component relative to the origin of the entity. For example, if you have a
        // 32x32 sprite and want a 48x48 overlap centered on the sprite, you would set this to (-8, -8).
        internal readonly Tuple<int, int> Offset = new Tuple<int, int>(0, 0);
        private readonly IList<Entity> currentlyOverlapping = new List<Entity>();

        private Action<Entity> onStartOverlap;
        private Action<Entity> onStopOverlap;

        /// <summary>
        /// Creates a mouse component (receives clicks and triggers a callback).
        /// Width and height indicate the overlap check area.
        /// Offsets indicate this component's offset relative to the origin of the entity.
        /// The overlap callbacks trigger when an entity newly starts/stop overlapping with us.
        /// </summary>
        public OverlapComponent(Entity parent, int width, int height, int offsetX = 0, int offsetY = 0, Action<Entity> onStartOverlap = null, Action<Entity> onStopOverlap = null)
        : base(parent)
        {
            this.Size = new Tuple<int, int>(width, height);
            this.Offset = new Tuple<int, int>(offsetX, offsetY);
            this.onStartOverlap = onStartOverlap;
            this.onStopOverlap = onStopOverlap;
        }

        public bool WasOverlapping(Entity target)
        {
            return this.currentlyOverlapping.Contains(target);
        }

        public bool IsOverlapping(Entity target)
        {
            var targetOverlap = target.GetIfHas<OverlapComponent>();
            if (targetOverlap == null)
            {
                return false;
            }

            // TODO: can we delegate this to MonoGame's Rectangle classes? Pretty please?
            // TODO: refactor and create variables for "absolute" top-left/bottom-right/etc. corners for this + other

            // We are out of range: either we're above/below or we're left/right of the target.
            if ((this.Parent.X < target.X - targetOverlap.Offset.Item1 || this.Parent.Y < target.Y - targetOverlap.Offset.Item2) ||
                (this.Parent.X - this.Offset.Item1 > target.X - targetOverlap.Offset.Item1 + targetOverlap.Size.Item1) ||
                (this.Parent.Y - this.Offset.Item2 > target.Y - targetOverlap.Offset.Item2 + targetOverlap.Size.Item2))
            {
                return false;
            }

            // https://tutorialedge.net/gamedev/aabb-collision-detection-tutorial/
            return ((this.Parent.X - this.Offset.Item1 < target.X - targetOverlap.Offset.Item1 + targetOverlap.Size.Item1) &&
                (this.Parent.X - this.Offset.Item1 + this.Size.Item1 > target.X - targetOverlap.Offset.Item1) &&
                (this.Parent.Y - this.Offset.Item2 < target.Y - targetOverlap.Offset.Item2 + targetOverlap.Size.Item2) &&
                (this.Parent.Y - this.Offset.Item2 + this.Size.Item2 > target.Y - targetOverlap.Offset.Item2));
        }

        public void StartedOverlapping(Entity target)
        {
            if (!this.currentlyOverlapping.Contains(target))
            {
                this.currentlyOverlapping.Add(target);

                if (this.onStartOverlap != null)
                {
                    this.onStartOverlap.Invoke(target);
                }
            }
        }

        public void StoppedOverlapping(Entity target)
        {
            if (this.currentlyOverlapping.Contains(target))
            {
                this.currentlyOverlapping.Remove(target);

                if (this.onStopOverlap != null)
                {
                    this.onStopOverlap.Invoke(target);
                }
            }
        }
    }
}