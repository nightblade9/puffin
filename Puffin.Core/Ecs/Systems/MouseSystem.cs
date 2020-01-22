using Ninject;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    public class MouseSystem : ISystem
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
            if (entity.GetIfHas<MouseComponent>() != null)
            {
                this.entities.Add(entity);
            }
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

            foreach (var entity in this.entities)
            {
                var mouse = entity.GetIfHas<MouseComponent>();

                if (clickedX >= entity.X && clickedY >= entity.Y && clickedX <= entity.X + mouse.Width && clickedY <= entity.Y + mouse.Height)
                {
                    mouse.OnClickCallback.Invoke();
                }
            }
        }
    }
}