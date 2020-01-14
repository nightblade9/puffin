using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    /// <summary>
    /// An entity; it holds components, which dictate behaviour (eg. add a
    // SpriteComponent to display a sprite wherever this entity is).
    /// Entities can only hold one component of each type.
    /// </summary>
    public class Entity : IDisposable
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

        /// <summary>
        /// Set/add a component on this entity. If this entity already had a 
        /// component of this type, this new component replaces the old one.
        /// </summary>
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

        public void Dispose()
        {
            // Remove callbacks so stuff can be GCed.
            this.OnPositionChangeCallbacks.Clear();
        }

        private void Remove(Type componentType)
        {
            if (this.components.ContainsKey(componentType))
            {
                this.components[componentType] = null;
                // Signal: component removed
            }
        }

        /// <summary>
        /// Triggers any subscribed callbacks that should fire when our position changes.
        /// </summary>
        private void TriggerOnPositionCallbacks()
        {
            foreach (var callback in this.OnPositionChangeCallbacks)
            {
                callback.Invoke(this.X, this.Y);
            }
        }
    }
}