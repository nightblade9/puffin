namespace Puffin.Core.Ecs.Components
{
    public class SpriteComponent : Component
    {
        public readonly string FileName;

        /// <summary>
        /// The width of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public readonly int FrameWidth;

        /// <summary>
        /// The height of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public readonly int FrameHeight;

        public SpriteComponent(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Used to construct a spritesheet, by passing in the frame width/height.
        /// </summary>
        public SpriteComponent(string spritesheetFileName, int frameWidth, int frameHeight)
        : this(spritesheetFileName)
        {
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
        }
    }
}