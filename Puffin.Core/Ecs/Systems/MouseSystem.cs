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
        private readonly EventBus eventBus;
        private readonly List<Entity> entities = new List<Entity>();

        private bool wasLeftButtonDown = false;
        
        public MouseSystem(EventBus eventBus, IMouseProvider mouseProvider)
        {
            this.eventBus = eventBus;
            this.provider = mouseProvider;
            eventBus.Subscribe(EventBusSignal.MouseReleased, this.OnMouseReleased);
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
            var isHandled = false;

            if (this.provider.IsLeftButtonDown && !wasLeftButtonDown)
            {
                foreach (var entity in this.entities)
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
                        isHandled |= mouse.OnClickCallback.Invoke(clickedX, clickedY);
                        
                    }

                    if (isHandled)
                    {
                        break;
                    }
                }

                // Not handled by any event handler so far; invoke the global ones.
                if (!isHandled)
                {
                    this.eventBus.Broadcast(EventBusSignal.MouseClicked);
                }                    
            }
            else if (!provider.IsLeftButtonDown && wasLeftButtonDown)
            {
                this.eventBus.Broadcast(EventBusSignal.MouseReleased);
            }

            this.wasLeftButtonDown = provider.IsLeftButtonDown;
        }

        private void OnMouseReleased(object data)
        {
            // ToArray prevents concurrent modification exceptions when we remove an entity on release.
            // Note that the performance is OK, since this is in response to an event; not every frame.
            foreach (var entity in this.entities.ToArray())
            {
                var mouse = entity.Get<MouseComponent>();
                if (mouse != null)
                {
                    mouse.OnReleaseCallback?.Invoke();
                }
            }
        }
    }
}