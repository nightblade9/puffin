using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Makes an entity check for overlap with other entities that have an <c>OverlapComponent</c>.
    /// </summary>
    public class OverlapComponent : Component
    {
        internal readonly Tuple<int, int> Size = new Tuple<int, int>(0, 0);
        internal readonly Tuple<int, int> Offset = new Tuple<int, int>(0, 0);
        internal readonly IList<Entity> CurrentlyOverlapping = new List<Entity>();

        internal Action<Entity> OnStartOverlap;
        internal Action<Entity> OnStopOverlap;

        /// <summary>
        /// Creates an overlap component, which checks for overlap against other entities with overlap components.
        /// </summary>
        /// <param name="width">The width of the overlap area, in pixels.</param>
        /// <param name="height">The height of the overlap area, in pixels.</param>
        /// <param name="offsetX">The x-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="offsetY">The y-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="onStartOverlap">The function to call when another entity's overlap component starts overlappin with this one, or null to do nothing.</param>
        /// <param name="onStopOverlap">The function to call when another entity's overlap component stops overlappin with this one, or null to do nothing.</param>
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