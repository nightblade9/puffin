using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.UI.Controls
{
    /// <summary>
    /// Creates a horizontal progress bar of the specified width (inner width).
    /// The progress bar is made up of an image, with the actual progress indicator
    /// set by a <c>ColourComponent</c> instance.
    /// <param name="paddingX">The X-axis padding between the image and the start of the colour/bar</param>
    /// <param name="paddingY">The Y-axis padding between the image and the start of the colour/bar</param>
    public class HorizontalProgressBar : Entity
    {
        private const int BAR_THICKNESS = 16;
        private readonly int paddingX = 0;
        private readonly int paddingY = 0;
        private readonly int maxValue;
        private int value = 0;

        public HorizontalProgressBar(string imageFileName, int barColourRgb, int innerWidth, int paddingX, int paddingY)
        {
            this.Sprite(imageFileName)
                .Colour(barColourRgb, innerWidth, BAR_THICKNESS, paddingX, paddingY);
            
            // Filled to start
            this.maxValue = innerWidth;
            this.value = this.maxValue;
        }

        public int Value {
            get { return this.value; }
            set {
                // TODO: validate the value is between 0 and max
                this.value = value;
                this.Get<ColourComponent>().Width = this.value;
            }
        }
    }
}