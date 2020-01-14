namespace Puffin.Core.Ecs.Components
{
    public class SpriteComponent : Component
    {
        public string FileName { get; private set; }

        /// <summary>
        /// The width of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public int FrameWidth = 0;

        /// <summary>
        /// The height of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public int FrameHeight = 0;

        public SpriteComponent(string fileName)
        {
            this.FileName = fileName;
        }
    }
}