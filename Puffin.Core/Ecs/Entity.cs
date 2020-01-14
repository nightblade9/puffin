using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    public class Entity
    {
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        private int x = 0;
        private int y = 0;
        private List<Action<int, int>> OnPositionChangeCallbacks = new List<Action<int, int>>();

        public int X {
            get { return x; }
            set { 
                x = value;
                this.TriggerOnPositionCallbacks();
            }
        }

        public int Y {
            get { return y; }
            set { 
                y = value;
                this.TriggerOnPositionCallbacks();
            }
        }
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

        public void AddPositionChangeCallback(Action<int, int> callback)
        {
            this.OnPositionChangeCallbacks.Add(callback);
        }

        public Entity Move(int x, int y)
        {
            this.X = x;
            this.Y = y;
            return this;
        }

        private void Remove(Type componentType)
        {
            if (this.components.ContainsKey(componentType))
            {
                this.components[componentType] = null;
                // Signal: component removed
            }
        }

        private void TriggerOnPositionCallbacks()
        {
            foreach (var callback in this.OnPositionChangeCallbacks)
            {
                callback.Invoke(this.X, this.Y);
            }
        }
    }
}