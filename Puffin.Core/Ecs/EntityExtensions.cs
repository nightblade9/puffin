namespace Puffin.Core.Ecs
{
    public static class EntityExtensions
    {
        public static Entity Move(this Entity entity, int x, int y)
        {
            entity.X = x;
            entity.Y = y;
            return entity;
        }
    }
}