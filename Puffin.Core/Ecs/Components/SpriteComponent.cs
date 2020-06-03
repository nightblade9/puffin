using System;
using Puffin.Core.Events;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a sprite/image (or spritesheet/tilesheet) to an entity.
    /// </summary>
    public class SpriteComponent : Component
    {
        /// <summary>The image file of this component.</summary>
        public string FileName
        {
            get { return this.fileName; }
            set
            {
                this.fileName = value;
                this.Parent.Scene?.EventBus.Broadcast(EventBusSignal.SpriteChanged, this);
            }
        }

        /// <summary>
        /// The width of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public readonly int FrameWidth;

        /// <summary>
        /// The height of a frame of the sprite (if it's a spritesheet), or zero if not a spritesheet.
        /// </summary>
        public readonly int FrameHeight;

        /// <summary>
        /// If false, doesn't display the sprite. See: <c>Alpha</c>
        /// </summary>
        public bool IsVisible = true;

        /// <summary>
        /// The alpha (transparency) of this sprite, ranging from 0 (invisible) to 1 (fully visible).
        /// See <c>IsVisible</c>.
        /// </summary>
        public float Alpha = 1;

        // Actual image size, set on load
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        private int frameIndex = 0;
        private string fileName;

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
                
                this.Parent.Scene?.EventBus.Broadcast(EventBusSignal.SpriteSheetFrameIndexChanged, this);
            }
        }

        /// <summary>Creates a new sprite.</summary>
        public SpriteComponent(Entity parent, string fileName)
        : base(parent)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Used to construct a spritesheet, where each frame has the specified width and height.
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