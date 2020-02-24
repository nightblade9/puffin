using System;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Tweening;

namespace Puffin.Core.Ecs
{
    /// <summary>
    /// Static extensions for Entity. You can chain them together, eg. Entity.Sprite("player.png").Move(200, 100);
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        ///  Immediately moves an entity to the specified coordinates, ignoring velocity/overlap/collision/etc.
        /// </summary>
        public static Entity Move(this Entity entity, int x, int y)
        {
            entity.X = x;
            entity.Y = y;
            return entity;
        }
        
        /// <summary>
        /// Allows an entity to play an audio file (short or long), optionally at a modified pitch.
        /// You should be able to play WAV files and OGG files.
        /// </summary>
        public static Entity Audio(this Entity entity, string audioFileName)
        {
            entity.Set(new AudioComponent(entity, audioFileName));
            return entity;
        }

        /// <summary>
        /// Adds a camera component to this entity, with the specified zoom.
        /// </summary>
        public static Entity Camera(this Entity entity, float zoom)
        {
            entity.Set(new CameraComponent(entity, zoom));
            return entity;
        }

        /// <summary>
        /// Causes an entity to collide with other collidable entities and solid tiles.
        /// </summary>
        /// <param name="width">The width of the collidable area, in pixels</param>
        /// <param name="height">The height of the collidable area, in pixels</param>
        /// <param name="slideOnCollide">If true, when colliding, slide in the direction of the non-colliding axis instead of abruptly stopping</param>
        /// <param name="xOffset">The x-offset of the collidable area relative to the origin of this entity</param>
        /// <param name="yOffset">The y-offset of the collidable area relative to the origin of this entity</param>
        public static Entity Collide(this Entity entity, int width, int height, bool slideOnCollide = false, int xOffset = 0, int yOffset = 0)
        {
            entity.Set(new CollisionComponent(entity, width, height, slideOnCollide, xOffset, yOffset));
            return entity;
        }

        /// <summary>
        /// Causes an entity to collide with other collidable entities and solid tiles.
        /// </summary>
        /// <param name="width">The width of the collidable area, in pixels</param>
        /// <param name="height">The height of the collidable area, in pixels</param>
        /// <param name="onCollide">A callback to invoke when colliding against another entity. The callback
        /// passes in the colliding entity and axis of collision ("X" or "Y") as parameters, and is invoked
        /// after resolving the collision.</param>
        public static Entity Collide(this Entity entity, int width, int height, Action<Entity, string> onCollide)
        {
            entity.Set(new CollisionComponent(entity, width, height, onCollide));
            return entity;
        }

        

        /// <summary>
        /// Adds a coloured rectangle to an entity.
        /// </summary>
        /// <param name="rgb">The rectangle's colour, in the format 0xRRGGBB with hex values for each pair.</param>
        public static Entity Colour(this Entity entity, int rgb, int width, int height, int offsetX = 0, int offsetY = 0)
        {
            entity.Set(new ColourComponent(entity, rgb, width, height, offsetX, offsetY));
            return entity;
        }

        /// <summary>
        /// Adds a component which makes the entity move in four directions in response to the keyboard.
        /// By default, this responds to the WASD and arrow keys; you can change these bindings
        /// by changing/adding more bindings in your PuffinGame instance.
        /// Note that setting this overrides an entity's velocity.
        /// </summary>
        public static Entity FourWayMovement(this Entity entity, int speed)
        {
            entity.Set(new FourWayMovementComponent(entity, speed));
            return entity;
        }
        
        /// <summary>
        /// Exposes a method that allows an entity to check/respond to actions/keys.
        /// </summary>
        /// <param name="onActionPressed">The function to invoke when an action's key is just pressed; the action is passed in as a parameter.</param>
        /// <param name="onActionReleased">The function to invoke when an action's key is just released; the action is passed in as a parameter.</param>
        /// <param name="onActionDown">The function to invoke when an action's key is held down.</param>
        public static Entity Keyboard(this Entity entity, Action<Enum> onActionPressed = null, Action<Enum> onActionReleased = null, Action<Enum> onActionDown = null)
        {
            entity.Set(new KeyboardComponent(entity, onActionPressed, onActionReleased, onActionDown));
            return entity;
        }

        /// <summary>
        /// Adds a text display to an entity.
        /// </summary>        
        /// <param name="offsetX">The X-offset of the label relative to the entity's origin</param>
        /// <param name="offsetY">The Y-offset of the label relative to the entity's origin</param>
        public static Entity Label(this Entity entity, string text, int offsetX = 0, int offsetY = 0)
        {
            entity.Set(new TextLabelComponent(entity, text, offsetX, offsetY));
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
        /// Creates an overlap component, which checks for overlap against other entities with overlap components.
        /// </summary>
        /// <param name="width">The width of the overlap area.</param>
        /// <param name="height">The height of the overlap area.</param>
        public static Entity Overlap(this Entity entity, int width, int height)
        {
            entity.Set(new OverlapComponent(entity, width, height));
            return entity;
        }

        /// <summary>
        /// Creates an overlap component, which checks for overlap against other entities with overlap components.
        /// </summary>
        /// <param name="width">The width of the overlap area.</param>
        /// <param name="height">The height of the overlap area.</param>
        /// <param name="offsetX">The x-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="offsetY">The y-offset of the overlap area, relative to the origin of the entity.</param>
        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY));
            return entity;
        }

        /// <summary>
        /// Creates an overlap component, which checks for overlap against other entities with overlap components.
        /// </summary>
        /// <param name="width">The width of the overlap area.</param>
        /// <param name="height">The height of the overlap area.</param>
        /// <param name="offsetX">The x-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="offsetY">The y-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="onStartOverlap">The callback to invoke when an entity with an overlap component overlaps this one.</param>
        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY, Action<Entity> onStartOverlap)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY, onStartOverlap));
            return entity;
        }

        /// <summary>
        /// Creates an overlap component, which checks for overlap against other entities with overlap components.
        /// </summary>
        /// <param name="width">The width of the overlap area.</param>
        /// <param name="height">The height of the overlap area.</param>
        /// <param name="offsetX">The x-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="offsetY">The y-offset of the overlap area, relative to the origin of the entity.</param>
        /// <param name="onStartOverlap">The callback to invoke when an entity with an overlap component overlaps this one.</param>
        /// <param name="onStopOverlap">The callback to invoke when an entity with an overlap component stops overlapping this one.</param>
        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY, Action<Entity> onStartOverlap, Action<Entity> onStopOverlap)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY, onStartOverlap, onStopOverlap));
            return entity;
        }

        /// <summary>
        /// Creates an overlap component, which checks for overlap against other entities with overlap components, and events for when
        /// the mouse starts/stops overlapping this entity's overlap region.
        /// </summary>
        /// <param name="width">The width of the overlap area.</param>
        /// <param name="height">The height of the overlap area.</param>
        /// <param name="onMouseEnter">The callback to invoke when the mouse enters the region occupied by this component.</param>
        /// <param name="onMouseExit">The callback to invoke when the mouse exits the region occupied by this component.</param>
        public static Entity Overlap(this Entity entity, int width, int height, int offsetX, int offsetY, Action onMouseEnter, Action onMouseExit = null)
        {
            entity.Set(new OverlapComponent(entity, width, height, offsetX, offsetY, null, null, onMouseEnter, onMouseExit));
            return entity;
        }
        
        /// <summary>
        /// Adds a sprite/image to an entity.
        /// </summary>
        public static Entity Sprite(this Entity entity, string imageFile)
        {
            entity.Set(new SpriteComponent(entity, imageFile));
            return entity;
        }

        /// <summary>
        /// Adds a spritesheet/tilesheet to an entity.
        /// </summary>
        public static Entity Spritesheet(this Entity entity, string imageFile, int frameWidth, int frameHeight, int frameIndex = 0)
        {
            entity.Set(new SpriteComponent(entity, imageFile, frameWidth, frameHeight, frameIndex));            
            return entity;
        }

        /// <summary>
        /// Adds a tween to an entity.
        /// </summary>
        /// <param name="entity">The entity to add the tween to.</param>
        /// <param name="startPosition">The position to move the entity to when the tween starts.</param>
        /// <param name="endPosition">The position the entity should occupy when the tween ends.</param>
        /// <param name="durationSeconds">How long the tween should take, in seconds.</param>
        /// <param name="onTweenComplete">An optional callback to invoke when the tween completes.</param>
        public static Entity Tween(this Entity entity, Tuple<float, float> startPosition, Tuple<float, float> endPosition, float durationSeconds, Action onTweenComplete = null)
        {
            TweenManager.LatestInstance.TweenPosition(entity, startPosition, endPosition, durationSeconds, onTweenComplete);
            return entity;
        }

        /// <summary>
        /// Sets an entity's velocity, causing it to move constantly in that direction.
        /// </summary>
        public static Entity Velocity(this Entity entity, int velocityX, int velocityY)
        {
            entity.VelocityX = velocityX;
            entity.VelocityY = velocityY;
            return entity;
        }
    }
}