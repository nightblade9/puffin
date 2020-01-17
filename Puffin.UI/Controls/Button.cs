using Puffin.Core.Ecs;
using System;

namespace Puffin.UI.Controls
{
    public class Button : Entity
    {
        private const int SPRITE_WIDTH = 128;
        private const int SPRITE_HEIGHT = 48;

        public Button(string text, Action onClick)
        {
            this.Sprite("Content/Puffin/UI/Button.png");
            this.Label(text);
            this.Mouse(onClick, SPRITE_WIDTH, SPRITE_HEIGHT);
        }
    }
}