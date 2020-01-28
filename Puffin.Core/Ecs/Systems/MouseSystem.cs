using Ninject;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    class MouseSystem : ISystem
    {
        private readonly IMouseProvider provider;
        private readonly List<Entity> entities = new List<Entity>();
        
        public MouseSystem()
        {
            this.provider = DependencyInjection.Kernel.Get<IMouseProvider>();            
            EventBus.LatestInstance.Subscribe(EventBusSignal.MouseClicked, this.onMouseClicked);
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

        private void onMouseClicked(object data)
        {
            // TODO: move this into mouse system
            var clickedX = provider.MouseCoordinates.Item1;
            var clickedY = provider.MouseCoordinates.Item2;

            // ToArray prevents concurrent modification exceptions when we remove an entity on click.
            // Note that the performance is OK, since this is in response to an event; not every frame.
            foreach (var entity in this.entities.ToArray())
            {
                var mouse = entity.Get<MouseComponent>();

                if (clickedX >= entity.X && clickedY >= entity.Y && clickedX <= entity.X + mouse.Width && clickedY <= entity.Y + mouse.Height)
                {
                    mouse.OnClickCallback.Invoke();
                }
            }
        }
    }
}