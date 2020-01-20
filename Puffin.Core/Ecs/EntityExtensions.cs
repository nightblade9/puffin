using System;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    public static class EntityExtensions
    {
        /// <summary>
        ///  Moves an entity to the specified coordinates.
        /// </summary>
        public static Entity Move(this Entity entity, int x, int y)
        {
            entity.X = x;
            entity.Y = y;
            return entity;
        }

        /// <summary>
        /// Loads the specified image and adds it as a sprite to the entity.
        /// </summary>
        public static Entity Sprite(this Entity entity, string imageFile)
        {
            entity.Set(new SpriteComponent(entity, imageFile));
            return entity;
        }

        /// <summary>
        /// Loads and adds the specified image as a spritesheet to the entity.
        /// </summary>
        public static Entity Spritesheet(this Entity entity, string imageFile, int frameWidth, int frameHeight)
        {
            entity.Set(new SpriteComponent(entity, imageFile, frameWidth, frameHeight));
            return entity;
        }

        /// <summary>
        /// Adds a label with the specified text to the entity.
        /// </summary>        
        public static Entity Label(this Entity entity, string text)
        {
            entity.Set(new TextLabelComponent(entity, text));
            return entity;
        }

        /// <summary>
        /// Adds a mouse component to the entity with the specified on-click callback.
        /// The width and height define the clickable area (relative to the origin of the entity).
        /// </summary>
        public static Entity Mouse(this Entity entity, Action onClick, int width, int height)
        {
            entity.Set(new MouseComponent(entity, onClick, width, height));
            return entity;
        }

        /// <summary>
        /// Adds a keyboard component to the entity so it can respond to actions/keys.
        /// </summary>
        public static Entity Keyboard(this Entity entity)
        {
            entity.Set(new KeyboardComponent(entity));
            return entity;
        }
        
        /// <summary>
        /// Adds a `FourWayMovement` component to the entity with the specified speed.
        /// (Speed is in pixels per second.)
        /// </summary>
        public static Entity FourWayMovement(this Entity entity, int speed)
        {
            entity.Set(new FourWayMovementComponent(entity, speed));
            return entity;
        }
    }
}