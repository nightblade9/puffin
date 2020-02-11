using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Makes an entity check for overlap with other entities that have an <c>OverlapComponent</c>.
    /// Can also be used to detect when a mouse enters/exits a region.
    /// </summary>
    public class OverlapComponent : Component
    {
        internal readonly Tuple<int, int> Size = new Tuple<int, int>(0, 0);
        internal readonly Tuple<int, int> Offset = new Tuple<int, int>(0, 0);
        internal readonly IList<Entity> CurrentlyOverlapping = new List<Entity>();

        internal Action<Entity> OnStartOverlap;
        internal Action<Entity> OnStopOverlap;
        internal Action OnMouseEnter;
        internal Action OnMouseExit;

        /// <summary>
        /// Creates an overlap component, which checks for overlap against other entities with overlap components.
        /// Can also detect when the mouse starts/stops overlapping this component's region.
        /// </summary>
        /// <param name="width">The width of the overlap area, in pixels.</param>
        /// <param name="height">The height of the overlap area, in pixels.</param>
        /// <param name="offsetX">The x-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="offsetY">The y-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="onStartOverlap">The function to call when another entity's overlap component starts overlappin with this one, or null to do nothing.</param>
        /// <param name="onStopOverlap">The function to call when another entity's overlap component stops overlappin with this one, or null to do nothing.</param>
        /// <param name="onMouseEnter">The function to call when the mouse starts overlapping this component, or null to do nothing.</param>
        /// <param name="onMouseExit">The function to call when the mouse stops overlapping this component, or null to do nothing.</param>
        public OverlapComponent(Entity parent, int width, int height, int offsetX = 0, int offsetY = 0,
            Action<Entity> onStartOverlap = null, Action<Entity> onStopOverlap = null,
            Action onMouseEnter = null, Action onMouseExit = null)
        : base(parent)
        {
            this.Size = new Tuple<int, int>(width, height);
            this.Offset = new Tuple<int, int>(offsetX, offsetY);
            this.OnStartOverlap = onStartOverlap;
            this.OnStopOverlap = onStopOverlap;
            this.OnMouseEnter = onMouseEnter;
            this.OnMouseExit = onMouseExit;
        }
    }
}