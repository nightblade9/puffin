using Puffin.Core.Ecs;
using System;

namespace Puffin.UI.Controls
{
    public class Button : Entity
    {
        private Action onClick;

        public Button(string text, Action onClick)
        {
            this.onClick = onClick;
            // Set sprite to some sprite thing
            // Set label caption to text
            // Set mouse component and bind to click event
        }
    }
}