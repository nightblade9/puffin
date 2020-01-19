using System;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using Ninject;

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

        public static Entity Mouse(this Entity entity, Action onClick, int width, int height)
        {
            entity.Set(new MouseComponent(entity, onClick, width, height));
            return entity;
        }

        public static Entity Keyboard(this Entity entity)
        {
            entity.Set(new KeyboardComponent(entity));
            return entity;
        }
    }
}