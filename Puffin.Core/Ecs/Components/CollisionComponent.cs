namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Indicates a component shold collide with both other entities that have
    /// collision components, as well as solid tiles.
    /// </summary>
    public class CollisionComponent : Component
    {
        public readonly int Width;
        public readonly int Height;

        /// <summary>
        /// When we collide against something, do we slide along it instead of abruptly stopping?
        /// </summary>
        public bool SlideOnCollide { get; set;}

        public CollisionComponent(Entity parent, int width, int height, bool slideOnCollide) : base(parent)
        {
            this.Width = width;
            this.Height = height;
            this.SlideOnCollide = slideOnCollide;
        }
    }
}