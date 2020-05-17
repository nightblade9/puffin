namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a coloured rectangle to an entity.
    /// </summary>
    public class ColourComponent : Component
    {
        /// <summary>The rectangle's colour, in the format 0xRRGGBB with hex values for each pair.</summary>
        public int Colour = 0xFFFFFF;
        /// <summary>The width of the colour component, in pixels.</summary>
        public int Width { get; set; }
        /// <summary>The height of the colour component, in pixels.</summary>
        public int Height { get; set; }
        /// <summary>The x-offset of the colour component relative to the origin of the entity.</summary>
        public int OffsetX { get; set; }
        /// <summary>The y-offset of the colour component relative to the origin of the entity.</summary>
        public int OffsetY { get; set; }
        
        /// <summary>
        /// How transparent this component is; ranges from 0 (fully invisible) to 1 (fully visible).
        /// </summary>
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