using Puffin.Core.Ecs.Components;

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

        public static Entity Image(this Entity entity, string imageFile)
        {
            entity.Set(new SpriteComponent(imageFile));
            return entity;
        }
    }
}