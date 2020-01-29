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

        /// <param name="rgb">The rectangle's colour, in the format 0xRRGGBB with hex values for each pair.</param>
        public ColourComponent(Entity entity, int rgb, int width, int height)
        : base(entity)
        {
            this.Colour = rgb;
            this.Width = width;
            this.Height = height;
        }
    }
}