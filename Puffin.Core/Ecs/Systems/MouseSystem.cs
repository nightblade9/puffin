using Ninject;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    class MouseSystem : ISystem
    {
        private readonly IMouseProvider provider;
        private readonly List<Entity> entities = new List<Entity>();
        
        public MouseSystem(EventBus eventBus, IMouseProvider mouseProvider)
        {
            this.provider = mouseProvider;
            eventBus.Subscribe(EventBusSignal.MouseClicked, this.OnMouseClicked);
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.Get<MouseComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
        }

        public void OnUpdate(TimeSpan elapsed)
        {
            // Not needed, we use the MouseClicked event to trigger checks and delegate to components
        }

        private void OnMouseClicked(object data)
        {
            // ToArray prevents concurrent modification exceptions when we remove an entity on click.
            // Note that the performance is OK, since this is in response to an event; not every frame.
            foreach (var entity in this.entities.ToArray())
            {
                int clickedX;
                int clickedY;

                if (entity.IsUiElement)
                {
                    clickedX = provider.UiMouseCoordinates.Item1;
                    clickedY = provider.UiMouseCoordinates.Item2;
                }
                else
                {
                    clickedX = provider.MouseCoordinates.Item1;
                    clickedY = provider.MouseCoordinates.Item2;
                }

                var mouse = entity.Get<MouseComponent>();

                if (clickedX >= entity.X && clickedY >= entity.Y && clickedX <= entity.X + mouse.Width && clickedY <= entity.Y + mouse.Height)
                {
                    mouse.OnClickCallback.Invoke();
                }
            }
        }
    }
}