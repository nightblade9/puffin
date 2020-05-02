using System;
using Puffin.Core;
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
        private const int BAR_THICKNESS = 12;
        private bool isDraggingHandle = false;
        private Action<int> onValueChanged;

        /// <summary>
        /// Creates a horizontal slider of the specified size, at the specified position.
        /// The slider is made up of an image for the handle, with a <c>ColourComponent</c> instance for the slider bar.
        /// Note that the handle starts at the entity position, and the bar is offset to the right and down.
        /// <param name="width">The width of the slider</param>
        /// <param name="minValue">The minimum value of the slider</param>
        /// <param name="maxValue">The maximum value of the slider</param>
        public HorizontalSlider(bool isUiElement, string handleImageFileName, int barColourRgb, int width, int minValue, int maxValue)
        : base(isUiElement)
        {
            this.DrawColourBeforeSprite = true;
            
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

            this.Colour(barColourRgb, width, BAR_THICKNESS).Sprite(handleImageFileName)
            // Can't get sprite height, dunno how big it is ... just buffer.
            .Mouse(width + 100, BAR_THICKNESS + 50, (x, y) =>
            {
                var sprite = this.Get<SpriteComponent>();
                var colour = this.Get<ColourComponent>();

                var clickedOnHandle =
                    x >= this.X + sprite.OffsetX && x <= this.X + sprite.OffsetX + sprite.Width &&
                    y >= this.Y + sprite.OffsetY && y <= this.Y + sprite.OffsetY + sprite.Height;
                
                // Assume handle height
                var clickedOnBar =
                    x >= this.X + colour.OffsetX && x <= this.X + colour.OffsetX + colour.Width &&
                    y >= this.Y + colour.OffsetY - (sprite.Height / 2) && y <= this.Y + colour.OffsetY + (sprite.Height / 2);

                float pixelsPerValue = this.width * 1f / this.maxValue;

                if (!clickedOnBar && !clickedOnHandle)
                {
                    return false;
                }

                if (!isDraggingHandle)
                {
                    if (clickedOnHandle)
                    {
                        this.isDraggingHandle = true;
                        Console.WriteLine("click");
                    }
                    else
                    {
                        // Move slider to selected location
                        this.UpdateValueTo(x);
                    }
                }
                else
                {
                    this.UpdateValueTo(x);
                }

                return true;
            },
            () => {
                this.isDraggingHandle = false;
            });

            this.OnUpdate(elapsed =>
            {
                if (this.isDraggingHandle)
                {
                    var scene = this.Scene;
                    var coordinates = isUiElement ? scene.UiMouseCoordinates : scene.MouseCoordinates;
                    var mouseX = coordinates.Item1;

                    if (mouseX >= this.X && mouseX <= this.X + this.Get<ColourComponent>().Width)
                    {
                        this.UpdateValueTo(mouseX);
                    }
                }
            });

            this.OnReady(() => {
                var colour = this.Get<ColourComponent>();
                var sprite = this.Get<SpriteComponent>();

                colour.OffsetX = sprite.Width / 2;
                colour.OffsetY = (sprite.Height - BAR_THICKNESS) / 2;
                this.RepositionHandle();
            });
            
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = this.minValue;
            this.width = width;
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

                if (value != this.value)
                {
                    this.onValueChanged?.Invoke(value);
                    this.value = value;
                    this.RepositionHandle();
                }
            }
        }

        public void OnValueChanged(Action<int> callback)
        {
            this.onValueChanged = callback;
        }

        private void UpdateValueTo(int mouseX)
        {
            float pixelsPerValue = this.width * 1f / this.maxValue;
            this.Value = (int)Math.Round((mouseX - this.X) / pixelsPerValue);
            this.RepositionHandle();
        }

        private void RepositionHandle()
        {
            var handle = this.Get<SpriteComponent>();
            var bar = this.Get<ColourComponent>();
            // 100 pixels, max value is 25, then ppv = 4
            float pixelsPerValue = this.width * 1f / this.maxValue;
            handle.OffsetX = (int)((pixelsPerValue * this.value));
        }
    }
}