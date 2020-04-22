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
        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="text">The text label on the button (if any).</param>
        /// <param name="onClick">The callback to trigger when the player clicks on the button.</param>
        public Button(bool isUiElement, string imagePath, int spriteWidth, int spriteHeight, string text, int textXOffset, int textYOffset, Func<int, int, bool> onClick)
        : base(isUiElement)
        {
            // Get rid of sprite width/height once this.Sprite(...) is enough to get the sprite width/height
            this.Sprite(imagePath);
            this.Label(text, textXOffset, textYOffset);
            this.Mouse(spriteWidth, spriteHeight, onClick);
        }
    }
}