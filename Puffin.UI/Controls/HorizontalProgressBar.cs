using System;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.UI.Controls
{
    /// <summary>
    /// A horizontal progress bar, such as a health bar; it uses a background sprite, and a colour component for the fill value.
    /// Note that this is just a pre-made collection of entities (sprite, colour, etc.).
    /// You can still <c>Get</c> them and do things like set the progress-bar colour.
    /// </summary>
    public class HorizontalProgressBar : Entity
    {
        private readonly int maxValue;
        private int value = 0;

        /// <summary>
        /// Creates a horizontal progress bar of the specified width (inner width).
        /// The progress bar is made up of an image, with the actual progress indicator
        /// set by a <c>ColourComponent</c> instance.
        /// </summary>
        /// <param name="isUiElement">True if this is a UI element (doesn't zoom with the camera).</param>
        /// <param name="imageFileName">The filename for the background image</param>
        /// <param name="barColourRgb">The colour of the progress bar</param>
        /// <param name="innerWidth">The width of the progress bar (progress part), not the image</param>
        /// <param name="barHeight">The height of the progress bar (progress part), not the image</param>
        /// <param name="paddingX">The X-axis padding between the image and the start of the colour/bar</param>
        /// <param name="paddingY">The Y-axis padding between the image and the start of the colour/bar</param>
        public HorizontalProgressBar(bool isUiElement, string imageFileName, int barColourRgb, int innerWidth, int barHeight, int paddingX, int paddingY)
        : base(isUiElement)
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                throw new ArgumentException(nameof(imageFileName));
            }
            if (innerWidth <= 0)
            {
                throw new ArgumentException("Please specify a positive inner-width");
            }
            if (barHeight <= 0)
            {
                throw new ArgumentException("The bar thickness must be positive");
            }

            this.Sprite(imageFileName)
                .Colour(barColourRgb, innerWidth, barHeight, paddingX, paddingY);
            
            // Filled to start
            this.maxValue = innerWidth;
            this.value = this.maxValue;
        }

        /// <summary>
        /// The value (filled progress part) of the bar; it must be between <c>0</c> and the value <c>innerWidth</c> specified in the constructor.
        /// </summary>
        public int Value {
            get { return this.value; }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > this.maxValue)
                {
                    value = this.maxValue;
                }

                this.value = value;
                this.Get<ColourComponent>().Width = this.value;
            }
        }
    }
}