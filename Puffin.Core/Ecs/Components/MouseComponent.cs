using System;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using Ninject;

namespace Puffin.Core.Ecs
{
    /// <summary>
    /// A component with makes an entity clickable.
    public class MouseComponent : Component
    {
        private readonly IMouseProvider mouseProvider;
        private readonly Action onClickCallback;
        
        // Clickable area width/height
        private readonly int width = 0;
        private readonly int height = 0;

        /// <summary>
        /// Creates a mouse component (receives clicks and triggers a callback).
        /// Width and height indicate the clickable area (relative to the origin of the entity).
        /// </summary>
        public MouseComponent(Entity parent, Action onClickCallback, int width, int height)
        : base(parent)
        {
            this.width = width;
            this.height = height;
            this.onClickCallback = onClickCallback;
            EventBus.LatestInstance.Subscribe(EventBusSignal.MouseClicked, this.onMouseClicked);
            this.mouseProvider = DependencyInjection.Kernel.Get<IMouseProvider>();
        }

        private void onMouseClicked(object data)
        {
            var clickedX = mouseProvider.MouseCoordinates.Item1;
            var clickedY = mouseProvider.MouseCoordinates.Item2;

            if (clickedX >= this.Parent.X && clickedY >= this.Parent.Y &&
                clickedX <= this.Parent.X + this.width && clickedY <= this.Parent.Y + this.height)
                {
                    this.onClickCallback.Invoke();
                }
        }
    }
}