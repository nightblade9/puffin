using System;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs
{
    /// <summary>
    /// Static extensions for Entity. You can chain them together, eg.
    /// Entity.Sprite("player.png").Move(200, 100);
    /// </summary>
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
        /// Loads the specified image as a sprite on an entity.
        /// </summary>
        public static Entity Sprite(this Entity entity, string imageFile)
        {
            entity.Set(new SpriteComponent(entity, imageFile));
            return entity;
        }

        /// <summary>
        /// Loads and adds the specified image as a spritesheet to an entity.
        /// </summary>
        public static Entity Spritesheet(this Entity entity, string imageFile, int frameWidth, int frameHeight)
        {
            entity.Set(new SpriteComponent(entity, imageFile, frameWidth, frameHeight));
            return entity;
        }

        /// <summary>
        /// Adds a label with the specified text to an entity.
        /// </summary>        
        public static Entity Label(this Entity entity, string text)
        {
            entity.Set(new TextLabelComponent(entity, text));
            return entity;
        }

        /// <summary>
        /// Allows an entity to handle mouse events; the specified callback will be invoked on every click.
        /// The width and height define the clickable area (relative to the origin of the entity).
        /// </summary>
        public static Entity Mouse(this Entity entity, Action onClick, int width, int height)
        {
            entity.Set(new MouseComponent(entity, onClick, width, height));
            return entity;
        }

        /// <summary>
        /// Exposes a method that allows an entity to check/respond to actions/keys.
        /// </summary>
        public static Entity Keyboard(this Entity entity)
        {
            entity.Set(new KeyboardComponent(entity));
            return entity;
        }
        
        /// <summary>
        /// Makes the entity move in four directions in response to WASD or arrow keys.
        /// The entity moves at the specified speed, in pixels per second.
        /// </summary>
        public static Entity FourWayMovement(this Entity entity, int speed)
        {
            entity.Set(new FourWayMovementComponent(entity, speed));
            return entity;
        }

        /// <summary>
        /// Causes an entity to trigger overlap events with other entities that have an overlap component.
        /// Width/height are the overlap area of this entity, relative to the origin.
        /// </summary>
        public static Entity Overlap(this Entity entity, int width, int height)
        {
            entity.Set(new OverlapComponent(entity, width, height));
            return entity;
        }

        /// <summary>
        /// Causes an entity to trigger overlap events with other entities that have an overlap component.
        /// Width/height are the overlap area of this entity, relative to the origin.
        /// Offset coordinates specify the offset of the overlap region relative to the origin of the entity.
        /// </summary>
        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY));
            return entity;
        }

        /// <summary>
        /// Causes an entity to trigger overlap events with other entities that have an overlap component.
        /// Width/height are the overlap area of this entity, relative to the origin.
        /// Offset coordinates specify the offset of the overlap region relative to the origin of the entity.
        /// onStartOverlap triggers whenever another entity with an overlap component starts overlapping this entity.
        /// </summary>
        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY, Action<Entity> onStartOverlap)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY, onStartOverlap));
            return entity;
        }

        /// <summary>
        /// Causes an entity to trigger overlap events with other entities that have an overlap component.
        /// Width/height are the overlap area of this entity, relative to the origin.
        /// Offset coordinates specify the offset of the overlap region relative to the origin of the entity.
        /// onStartOverlap triggers whenever another entity with an overlap component starts overlapping this entity.
        /// onStopOverlap triggers whenever another entity with an overlap component stops overlapping overlaps this entity.
        /// </summary>

        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY, Action<Entity> onStartOverlap, Action<Entity> onStopOverlap)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY, onStartOverlap, onStopOverlap));
            return entity;
        }

        /// <summary>
        /// Adds an audio file to an entity; you can call it via e.GetIfHas<AudioComponent>().Play(pitch).
        /// You should be able to play wave files and OGG files.
        /// For more information/arguments, see the AudioComponent docs.
        /// </summary>
        public static Entity Audio(this Entity entity, string audioFileName)
        {
            entity.Set(new AudioComponent(entity, audioFileName));
            return entity;
        }

        /// <summary>
        /// Adds a coloured rectangle to an entity with the specified size and colour.
        /// The colour format is RGB, eg. 0x0088FF for a light sky blue.
        /// </summary>
        public static Entity Colour(this Entity entity, uint rgb, int width, int height)
        {
            entity.Set(new ColourComponent(entity, rgb, width, height));
            return entity;
        }
    }
}