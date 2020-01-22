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
        internal readonly IList<Entity> CurrentlyOverlapping = new List<Entity>();

        internal Action<Entity> OnStartOverlap;
        internal Action<Entity> OnStopOverlap;

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
            this.OnStartOverlap = onStartOverlap;
            this.OnStopOverlap = onStopOverlap;
        }
    }
}