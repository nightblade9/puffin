namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Makes an entity collide with solid tiles and other entities that have a <c>CollisionComponent</c>
    /// </summary>
    public class CollisionComponent : Component
    {
        public readonly int Width;
        public readonly int Height;

        /// <summary>
        /// If true, when colliding, slide in the direction of the non-colliding axis instead of abruptly stopping.
        /// </summary>
        public bool SlideOnCollide { get; set;}

        /// <param name="width">The width of the collidable area, in pixels</param>
        /// <param name="height">The height of the collidable area, in pixels</param>
        /// <param name="slideOnCollide">If true, when colliding, slide in the direction of the non-colliding
        /// axis instead of abruptly stopping.</param>
        public CollisionComponent(Entity parent, int width, int height, bool slideOnCollide) : base(parent)
        {
            this.Width = width;
            this.Height = height;
            this.SlideOnCollide = slideOnCollide;
        }
    }
}