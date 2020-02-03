using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Makes an entity collide with solid tiles and other entities that have a <c>CollisionComponent</c>
    /// </summary>
    public class CollisionComponent : Component
    {
        public readonly int Width;
        public readonly int Height;
        internal Action<Entity, string> onCollide;

        /// <summary>
        /// If true, when colliding, slide in the direction of the non-colliding axis instead of abruptly stopping.
        /// </summary>
        public bool SlideOnCollide { get; set;}

        /// <param name="width">The width of the collidable area, in pixels</param>
        /// <param name="height">The height of the collidable area, in pixels</param>
        /// <param name="slideOnCollide">If true, when colliding, slide in the direction of the non-colliding
        /// axis instead of abruptly stopping.</param>
        public CollisionComponent(Entity parent, int width, int height, bool slideOnCollide)
        : this(parent, width, height, null)
        {
            this.SlideOnCollide = slideOnCollide;
        }

        /// <param name="width">The width of the collidable area, in pixels</param>
        /// <param name="height">The height of the collidable area, in pixels</param>
        /// <param name="onCollide">A callback to invoke when colliding against another entity. The callback
        /// passes in the colliding entity and axis of collision ("X" or "Y") as parameters, and is invoked
        /// after resolving the collision.</param>
        public CollisionComponent(Entity parent, int width, int height, Action<Entity, string> onCollide)
        : base(parent)
        {
            this.Width = width;
            this.Height = height;
            this.onCollide = onCollide;
        }
    }
}