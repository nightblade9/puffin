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
        internal readonly Action OnReleaseCallback;
        // Clickable area width/height
        internal readonly int Width = 0;
        internal readonly int Height = 0;

        /// <summary>
        /// Creates a mouse component (receives clicks and triggers a callback).
        /// Width and height indicate the clickable area (relative to the origin of the entity).
        /// </summary>
        public MouseComponent(Entity parent, int width, int height, Action<int, int> onClickCallback, Action onReleaseCallback = null)
        : base(parent)
        {
            this.Width = width;
            this.Height = height;
            this.OnClickCallback = onClickCallback;
            this.OnReleaseCallback = onReleaseCallback;
        }
    }
}