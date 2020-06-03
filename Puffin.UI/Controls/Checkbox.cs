using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using System;
using System.IO;

namespace Puffin.UI.Controls
{
    /// <summary>
    /// Creates a clickable checkbox. The checkbox uses two different images, one to show when it's checked and
    /// the other when unchecked. The control is not checked by default.
    /// </summary>
    public class Checkbox : Entity
    {
        /// <summary>Toggles the checked state of this control.</summary>
        public bool IsChecked
        {
            get { return this.isChecked; }
            set
            {
                this.isChecked = value;
                this.Get<SpriteComponent>().FileName = this.isChecked ? this.checkedImage : this.uncheckedImage;
                this.onToggle?.Invoke();
            }
        }

        // TODO: just take one sprite and assume it is two frames (divide with by 2).
        private readonly string uncheckedImage;
        private readonly string checkedImage;
        private readonly Action onToggle;
        private bool isChecked = false;

        /// <summary>
        /// Creates a new checkbox.
        /// </summary>
        /// <param name="isUiElement">True if this is a UI element (doesn't zoom with the camera).</param>
        /// <param name="uncheckedImage">The path to the image to show when the control is not checked.</param>
        /// <param name="checkedImage">The path to the image to show when the control is checked.</param>
        /// <param name="spriteHeight">The height of the button image sprite.</param>
        /// <param name="text">The text label next to the checkbox (if any).</param>
        /// <param name="onToggle">The callback to trigger when the player checks/unchecks the box.</param>
        public Checkbox(bool isUiElement, string uncheckedImage, string checkedImage, int spriteWidth, int spriteHeight, string text, Action onToggle = null)
        : base(isUiElement)
        {
            this.uncheckedImage = uncheckedImage;
            this.checkedImage = checkedImage;
            
            // Get rid of sprite width/height once this.Sprite(...) is enough to get the sprite width/height.
            // ALSO: use a spritesheet with two frames, that's easier to use.
            this.Sprite(checkedImage);
            this.Mouse(spriteWidth, spriteHeight, (x, y) => {
                this.IsChecked = !this.IsChecked;
                return false;
            });
            this.Label(text, spriteWidth + 8);
            this.IsChecked = true;

            // Don't fire when we set IsChecked just above, could result in null pointer exception in user code
            /// depending on where in Puffin's game/scene life-cycle we're in.
            this.onToggle = onToggle;
        }
    }
}