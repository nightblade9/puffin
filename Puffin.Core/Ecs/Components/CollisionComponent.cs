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

        public CollisionComponent(Entity parent, int width, int height) : base(parent)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}