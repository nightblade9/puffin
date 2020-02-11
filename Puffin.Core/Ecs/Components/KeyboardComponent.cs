using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A keyboard component, with handlers for actions (which map to a one or more keyboard keys).
    /// </summary>
    public class KeyboardComponent : Component
    {
        internal Action<Enum> OnActionPressed;
        internal Action<Enum> OnActionReleased;

        /// <param name="onActionPressed">The function to invoke when an action's key is just pressed; the action is passed in as a parameter.</param>
        /// <param name="onActionReleased">The function to invoke when an action's key is just released; the action is passed in as a parameter.</param>
        public KeyboardComponent(Entity parent, Action<Enum> onActionPressed = null, Action<Enum> onActionReleased = null) : base(parent)
        {
            this.OnActionPressed = onActionPressed;
            this.OnActionReleased = onActionReleased;
        }
    }
}