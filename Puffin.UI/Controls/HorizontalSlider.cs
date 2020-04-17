using System;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.UI.Controls
{
    public class HorizontalSlider : Entity
    {
        private readonly int minValue;
        private readonly int maxValue;
        private readonly int width;
        private int value = 0;
        private const int BAR_THICKNESS = 8;

        /// <summary>
        /// Creates a horizontal slider of the specified size, at the specified position.
        /// The slider is made up of an image for the handle, with a <c>ColourComponent</c> instance for the slider bar.
        /// Note that the slider bar is exactly the specified with, and the handle overflows by width/2 when it's at the
        /// minimum or maximum value.
        /// <param name="width">The width of the slider</param>
        /// <param name="minValue">The minimum value of the slider</param>
        /// <param name="maxValue">The maximum value of the slider</param>
        public HorizontalSlider(bool isUiElement, string handleImageFileName, int barColourRgb, int width, int minValue, int maxValue)
        : base(isUiElement)
        {
            if (string.IsNullOrWhiteSpace(handleImageFileName))
            {
                throw new ArgumentException(nameof(handleImageFileName));
            }
            if (width <= 0)
            {
                throw new ArgumentException("Please specify a positive width");
            }
            if (maxValue <= minValue)
            {
                throw new ArgumentException("Maximum value must be more than minimum value");
            }

            this.Sprite(handleImageFileName).Colour(barColourRgb, width, BAR_THICKNESS);
            
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = this.minValue;
            this.width = width;
        }

        override public void OnReady()
        {
            base.OnReady();
            var colour = this.Get<ColourComponent>();
            var sprite = this.Get<SpriteComponent>();
            this.RepositionHandle();
            sprite.OffsetY = -(sprite.Height - BAR_THICKNESS) / 2;
        }

        public int Value
        {
            get { return this.value; }
            set {
                if (value < minValue)
                {
                    value = minValue;
                }
                else if (value > this.maxValue)
                {
                    value = this.maxValue;
                }

                this.value = value;
                this.RepositionHandle();
            }
        }

        private void RepositionHandle()
        {
            var handle = this.Get<SpriteComponent>();
            var bar = this.Get<ColourComponent>();
            // 100 pixels, max value is 25, then ppv = 4
            float pixelsPerValue = this.width * 1f / this.maxValue;
            handle.OffsetX = (int)((pixelsPerValue * this.value)) - (handle.Width / 2);
        }
    }
}