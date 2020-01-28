using System;

namespace Puffin.Core.Ecs.Components
{
    public class ColourComponent : Component
    {
        /// <summary>0xRRGGBB</summary>
        public int Colour = 0xFFFFFF;
        public int Width { get; set; }
        public int Height { get; set; }

        // TODO: validate if colour is out of range
        public ColourComponent(Entity entity, int rgb, int width, int height)
        : base(entity)
        {
            this.Colour = rgb;
            this.Width = width;
            this.Height = height;
        }
    }
}