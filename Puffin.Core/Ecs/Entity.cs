using System;
using System.Collections.Generic;

using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    public class Entity
    {
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

        public Entity Set(Component component)
        {
            var type = component.GetType();
            this.Remove(type); // Remove if present, may trigger event
            this.components[type] = component;
            return this; // for chaining calls
        }

        public void Remove<T>() where T : Component
        {
            var type = typeof(T);
            this.Remove(type);
        }

        public T GetIfHas<T>() where T : Component
        {
            var type = typeof(T);
            if (this.components.ContainsKey(type))
            {
                return (T)this.components[type];
            }

            return null;
        }

        private void Remove(Type componentType)
        {
            if (this.components.ContainsKey(componentType))
            {
                this.components[componentType] = null;
                // Signal: component removed
            }
        }
    }
}