using Puffin.Core.Ecs.Components;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    class KeyboardSystem : ISystem
    {
        private readonly List<Entity> entities = new List<Entity>();
        
        public KeyboardSystem()
        {
            EventBus.LatestInstance.Subscribe(EventBusSignal.ActionPressed, this.OnActionPressed);
            EventBus.LatestInstance.Subscribe(EventBusSignal.ActionReleased, this.OnActionReleased);
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.Get<KeyboardComponent>() != null)
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
            // Not needed, we use the ActionPressed and ActionReleased events to trigger checks and delegate to components
        }

        private void OnActionPressed(object data)
        {
            Enum action = data as Enum;
            foreach (var entity in this.entities.ToArray())
            {
                var keyComponent = entity.Get<KeyboardComponent>();
                keyComponent.OnActionPressed?.Invoke(action);
            }
        }

        private void OnActionReleased(object data)
        {
            Enum action = data as Enum;
            foreach (var entity in this.entities.ToArray())
            {
                var keyComponent = entity.Get<KeyboardComponent>();
                keyComponent.OnActionReleased?.Invoke(action);
            }
        }
    }
}