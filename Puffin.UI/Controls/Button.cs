using Puffin.Core.Ecs;
using System;
using System.IO;

namespace Puffin.UI.Controls
{
    /// <summary>
    /// Creates a clickable button backed with specified image and caption.
    /// Note that this is just a pre-made collection of entities (sprite, mouse, label, etc.).
    /// You can still <c>Get</c> them and do things like set the font and/or font size.
    /// </summary>
    public class Button : Entity
    {
        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="isUiElement">True if this is a UI element (doesn't zoom with the camera).</param>
        /// <param name="imagePath">The path to the button image.</param>
        /// <param name="spriteWidth">The width of the button sprite image.</param>
        /// <param name="spriteHeight">The height of the button image sprite.</param>
        /// <param name="text">The text label on the button (if any).</param>
        /// <param name="textXOffset">The text label y-offset relative to the origin of the button entity.</param>
        /// <param name="textYOffset">The text label y-offset relative to the origin of the button entity.</param>
        /// <param name="onClick">The callback to trigger when the player clicks on the button.</param>
        public Button(bool isUiElement, string imagePath, int spriteWidth, int spriteHeight, string text, int textXOffset, int textYOffset, Func<int, int, bool> onClick)
        : base(isUiElement)
        {
            // Get rid of sprite width/height once this.Sprite(...) is enough to get the sprite width/height
            this.Sprite(imagePath);
            this.Label(text, textXOffset, textYOffset);
            // TODO: could be cleaner if we had a way of deferring this to OnReady; then we could just .Get<Sprite>().Width
            this.Mouse(spriteWidth, spriteHeight, onClick);
        }
    }
}