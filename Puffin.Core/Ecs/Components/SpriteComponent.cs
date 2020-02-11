using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a sprite/image (or spritesheet/tilesheet) to an entity.
    /// </summary>
    public class SpriteComponent : Component
    {
        /// <summary>The image file of this component.</summary>
        public readonly string FileName;

        /// <summary>
        /// The width of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public readonly int FrameWidth;

        /// <summary>
        /// The height of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public readonly int FrameHeight;

        /// <summary>
        /// If false, doesn't display the sprite.
        /// </summary>
        public bool IsVisible = true;

        private int frameIndex = 0;

        /// <summary>
        /// The frame index of the sprite; note that Puffin only currently supports a single row of frames.
        /// </summary>
        public int FrameIndex
        { 
            get { return this.frameIndex; }
            set
            {
                if (this.FrameWidth == 0 || this.FrameHeight == 0)
                {
                    throw new ArgumentException("Can't set frame index without a frame width/height");
                }

                if (value < 0)
                {
                    throw new ArgumentException("Frame index must be non-negative");
                }

                this.frameIndex = value;
                
                EventBus.LatestInstance.Broadcast(EventBusSignal.SpriteSheetFrameIndexChanged, this);
            }
        }

        public SpriteComponent(Entity parent, string fileName)
        : base(parent)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Used to construct a spritesheet, by passing in the frame width/height.
        /// </summary>
        public SpriteComponent(Entity parent, string spritesheetFileName, int frameWidth, int frameHeight, int frameIndex = 0)
        : this(parent, spritesheetFileName)
        {
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.FrameIndex = frameIndex;
        }
    }
}