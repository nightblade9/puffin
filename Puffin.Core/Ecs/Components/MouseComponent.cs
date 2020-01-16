using System;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;

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

        public MouseComponent(Entity parent, IMouseProvider mouseProvider, Action onClickCallback, int width, int height)
        : base(parent)
        {
            this.mouseProvider = mouseProvider;
            this.width = width;
            this.height = height;
            this.onClickCallback = onClickCallback;
            EventBus.LatestInstance.Subscribe(EventBusSignal.MouseClicked, this.onMouseClicked);
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