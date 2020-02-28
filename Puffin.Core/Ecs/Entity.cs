using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    /// <summary>
    /// An entity; it holds components, which dictate behaviour (eg. add a
    /// SpriteComponent to display a sprite wherever this entity is).
    /// Entities can only hold one component of each type.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The x-coordinate of this entity (positive x moves right).
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The y-coordinate of this entity (positive y moves down).
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The x-velocity of this entity, in pixels per second.
        /// </summary>
        public float VelocityX { get; set; }

        /// <summary>
        /// The y-velocity of this entity, in pixels per second.
        /// </summary>
        public float VelocityY { get; set; }

        // Used by the collision system; when set, the entity "intends" to move to this location.
        // Pending, of course, successful collision resolution checks (if it has a collision component).
        internal float IntendedMoveDeltaX = 0;
        internal float IntendedMoveDeltaY = 0;

        // If true, this entity is drawn above/after everything else, and ignores the camera.
        internal readonly bool IsUiElement = false;

        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

        internal List<Action<float>> OnUpdateActions = new List<Action<float>>();

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="isUiElement">True if we are using this entity to draw UI elements (above all other elements, and
        /// ignoring the camera).</param>
        public Entity(bool isUiElement = false)
        {
            this.IsUiElement = isUiElement;
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

        /// <summary>
        /// Removes a component of a specific type, if the entity has one of those. (Doesn't throw if not.)
        /// </summary>
        public void Remove<T>() where T : Component
        {
            var type = typeof(T);
            this.Remove(type);
        }

        /// <summary>
        /// Get the component of the specified type, if it exists; returns null if not.
        /// </summary>
        public T Get<T>() where T : Component
        {
            var type = typeof(T);
            if (this.components.ContainsKey(type))
            {
                return (T)this.components[type];
            }

            return null;
        }

        /// <summary>
        /// Add a new action that should trigger every update (of game logic).
        /// Note that you may get several updates before we redraw everything.
        /// The action receives the elapsed time (since the last update) in
        /// milliseconds as an input.
        /// </summary>
        public void OnUpdate(Action<float> action)
        {
            this.OnUpdateActions.Add(action);
        }

        private void Remove(Type componentType)
        {
            if (this.components.ContainsKey(componentType))
            {
                this.components[componentType] = null;
            }
        }
    }
}