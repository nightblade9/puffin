using System;

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

        private int frameIndex = 0;

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