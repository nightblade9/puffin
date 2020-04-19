namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a coloured rectangle to an entity.
    /// </summary>
    public class ColourComponent : Component
    {
        /// <summary>The rectangle's colour, in the format 0xRRGGBB with hex values for each pair.</summary>
        public int Colour = 0xFFFFFF;
        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        
        /// <summary>How transparent this component is; ranges from 0 (fully invisible) to 1 (fully visible).
        public float Alpha { get; set; } = 1;

        /// <param name="rgb">The rectangle's colour, in the format 0xRRGGBB with hex values for each pair.</param>
        public ColourComponent(Entity entity, int rgb, int width, int height, int offsetX = 0, int offsetY = 0)
        : base(entity)
        {
            this.Colour = rgb;
            this.Width = width;
            this.Height = height;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }
    }
}