using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs.Systems
{
    class KeyboardSystem : ISystem
    {
        private readonly List<Entity> entities = new List<Entity>();
        private readonly IKeyboardProvider keyboardProvider;
        // Keep a list of keys when they're pressed-down and remove when released
        private readonly List<Enum> keysDown = new List<Enum>();
        
        public KeyboardSystem(EventBus eventBus, IKeyboardProvider keyboardProvider)
        {
            this.keyboardProvider = keyboardProvider;
            eventBus.Subscribe(EventBusSignal.ActionPressed, this.OnActionPressed);
            eventBus.Subscribe(EventBusSignal.ActionReleased, this.OnActionReleased);
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
            foreach (var entity in this.entities)
            {
                var actionDownCallback = entity.Get<KeyboardComponent>()?.OnActionDown;
                if (actionDownCallback != null)
                {
                    foreach (var key in this.keysDown)
                    {
                        actionDownCallback.Invoke(key);
                    }
                }
            }
        }

        private void OnActionPressed(object data)
        {
            Enum action = data as Enum;
            this.keysDown.Add(action);
            foreach (var entity in this.entities.ToArray())
            {
                var keyComponent = entity.Get<KeyboardComponent>();
                keyComponent.OnActionPressed?.Invoke(action);
            }
        }

        private void OnActionReleased(object data)
        {
            Enum action = data as Enum;
            this.keysDown.Remove(action);
            foreach (var entity in this.entities.ToArray())
            {
                var keyComponent = entity.Get<KeyboardComponent>();
                keyComponent.OnActionReleased?.Invoke(action);
            }
        }
    }
}