using System;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;

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

        public static Entity Sprite(this Entity entity, string imageFile)
        {
            entity.Set(new SpriteComponent(entity, imageFile));
            return entity;
        }

        public static Entity Spritesheet(this Entity entity, string imageFile, int frameWidth, int frameHeight)
        {
            entity.Set(new SpriteComponent(entity, imageFile, frameWidth, frameHeight));
            return entity;
        }

        public static Entity Label(this Entity entity, string text)
        {
            entity.Set(new TextLabelComponent(entity, text));
            return entity;
        }

        public static Entity Mouse(this Entity entity, IMouseProvider mouseProvider, Action onClick, int width, int height)
        {
            // TODO: DI for mouse provider (singleton instance)
            entity.Set(new MouseComponent(entity, mouseProvider, onClick, width, height));
            return entity;
        }
    }
}