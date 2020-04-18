using Puffin.Core.Ecs;
using System;
using System.IO;

namespace Puffin.UI.Controls
{
    /// <summary>
    /// Creates a clickable button backed with specified image and caption.
    /// By default, uses the image in <c>Content/Puffin/UI/Button.png</c> as its image.
    /// </summary>
    public class Button : Entity
    {
        private const int SPRITE_WIDTH = 128;
        private const int SPRITE_HEIGHT = 48;

        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="text">The text label on the button (if any).</param>
        /// <param name="onClick">The callback to trigger when the player clicks on the button.</param>
        public Button(bool isUiElement, string imagePath, string text, int textXOffset, int textYOffset, Action<int, int> onClick)
        : base(isUiElement)
        {
            this.Sprite(imagePath);
            this.Label(text, textXOffset, textYOffset);
            this.Mouse(onClick, SPRITE_WIDTH, SPRITE_HEIGHT);
        }
    }
}