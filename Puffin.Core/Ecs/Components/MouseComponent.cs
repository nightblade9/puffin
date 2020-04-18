using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A component with makes an entity clickable.
    /// </summary>
    public class MouseComponent : Component
    {
        // To avoid having an `Update` method on the component, we rely on events.

        internal readonly Action<int, int> OnClickCallback;
        // Clickable area width/height
        internal readonly int Width = 0;
        internal readonly int Height = 0;

        /// <summary>
        /// Creates a mouse component (receives clicks and triggers a callback).
        /// Width and height indicate the clickable area (relative to the origin of the entity).
        /// </summary>
        public MouseComponent(Entity parent, Action<int, int> onClickCallback, int width, int height)
        : base(parent)
        {
            this.Width = width;
            this.Height = height;
            this.OnClickCallback = onClickCallback;
        }
    }
}