using System;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.UI.Controls
{
    public class HorizontalProgressBar : Entity
    {
        private const int BAR_THICKNESS = 16;
        private readonly int maxValue;
        private int value = 0;

        /// <summary>
        /// Creates a horizontal progress bar of the specified width (inner width).
        /// The progress bar is made up of an image, with the actual progress indicator
        /// set by a <c>ColourComponent</c> instance.
        /// <param name="innerWidth">The width of the progress bar (progress part), not the image</param>
        /// <param name="paddingY">The Y-axis padding between the image and the start of the colour/bar</param>
        /// <param name="paddingY">The Y-axis padding between the image and the start of the colour/bar</param>
        public HorizontalProgressBar(string imageFileName, int barColourRgb, int innerWidth, int paddingX, int paddingY)
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                throw new ArgumentException(nameof(imageFileName));
            }
            if (innerWidth <= 0)
            {
                throw new ArgumentException("Please specify a positive inner-width");
            }

            this.Sprite(imageFileName)
                .Colour(barColourRgb, innerWidth, BAR_THICKNESS, paddingX, paddingY);
            
            // Filled to start
            this.maxValue = innerWidth;
            this.value = this.maxValue;
        }

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