using Ninject;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puffin.Core.Ecs.Systems
{
    class MouseSystem : ISystem
    {
        private readonly IMouseProvider provider;
        private readonly EventBus eventBus;
        private readonly List<Entity> entities = new List<Entity>();

        private readonly Dictionary<ClickType, bool> wasButtonDown = new Dictionary<ClickType, bool>();
        
        public MouseSystem(EventBus eventBus, IMouseProvider mouseProvider)
        {
            this.eventBus = eventBus;
            this.provider = mouseProvider;
            eventBus.Subscribe(EventBusSignal.MouseReleased, this.OnMouseReleased);

            foreach (var clickType in Enum.GetValues(typeof(ClickType)).Cast<ClickType>())
            {
                wasButtonDown[clickType] = false;
            }
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
            var clickTypes = Enum.GetValues(typeof(ClickType)).Cast<ClickType>();
            foreach (var clickType in clickTypes)
            {
                if (this.provider.IsButtonDown(clickType) && !wasButtonDown[clickType])
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
                            isHandled |= mouse.OnClickCallback.Invoke(clickedX, clickedY, clickType);
                            
                        }

                        if (isHandled)
                        {
                            break;
                        }
                    }

                    // Not handled by any event handler so far; invoke the global ones.
                    if (!isHandled)
                    {
                        this.eventBus.Broadcast(EventBusSignal.MouseClicked, clickType);
                    }                    
                }
                else if (!provider.IsButtonDown(clickType) && wasButtonDown[clickType])
                {
                    this.eventBus.Broadcast(EventBusSignal.MouseReleased, clickType);
                }

                this.wasButtonDown[clickType] = provider.IsButtonDown(clickType);
            }
        }

        private void OnMouseReleased(object data)
        {
            var clickType = (ClickType)data;
            // ToArray prevents concurrent modification exceptions when we remove an entity on release.
            // Note that the performance is OK, since this is in response to an event; not every frame.
            foreach (var entity in this.entities.ToArray())
            {
                var mouse = entity.Get<MouseComponent>();
                if (mouse != null)
                {
                    mouse.OnReleaseCallback?.Invoke(clickType);
                }
            }
        }
    }
}