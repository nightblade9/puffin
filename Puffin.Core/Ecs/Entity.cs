using System;
using System.Collections.Generic;

using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    public class Entity
    {
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

        public void Add(Component component)
        {
            var type = component.GetType();
            this.Remove(type);
        }

        public void Remove(Type type)
        {
            if (this.components.ContainsKey(type))
            {
                this.components[type] = null;
                // Signal: component removed
            }
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
    }
}