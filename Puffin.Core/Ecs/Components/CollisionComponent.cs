using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Makes an entity collide with solid tiles and other entities that have a <c>CollisionComponent</c>
    /// </summary>
    public class CollisionComponent : Component
    {
        /// <summary>The width of the collision component (collidable area).</summary>
        public readonly int Width;
        /// <summary>The height of the collision component (collidable area).</summary>
        public readonly int Height;
        /// <summary>The x-offset of the collision component relative to the origin of the entity.</summary>
        public readonly int XOffset;
        /// <summary>The y-offset of the collision component relative to the origin of the entity.</summary>
        public readonly int YOffset;
        
        internal readonly Action<Entity, string> onCollide;

        /// <summary>
        /// If true, when colliding, slide in the direction of the non-colliding axis instead of abruptly stopping.
        /// </summary>
        public bool SlideOnCollide { get; set;}

        /// <param name="width">The width of the collidable area, in pixels</param>
        /// <param name="height">The height of the collidable area, in pixels</param>
        /// <param name="slideOnCollide">If true, when colliding, slide in the direction of the non-colliding axis instead of abruptly stopping</param>
        /// <param name="xOffset">The x-offset of the collidable area relative to the origin of this entity</param>
        /// <param name="yOffset">The y-offset of the collidable area relative to the origin of this entity</param>
        public CollisionComponent(Entity parent, int width, int height, bool slideOnCollide, int xOffset, int yOffset)
        : this(parent, width, height, null)
        {
            this.SlideOnCollide = slideOnCollide;
            this.XOffset = xOffset;
            this.YOffset = yOffset;
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