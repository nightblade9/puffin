using System;
using Microsoft.Xna.Framework.Input;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using Puffin.Core.IO;

namespace Puffin.Infrastructure.MonoGame.IO
{
    /// <summary>
    /// An implementation of IMouseProvider using MonoGame's `Mouse` class.
    /// </summary>
    class MonoGameMouseProvider : IMouseProvider
    {
        private MouseState previousState;

        public Tuple<int, int> MouseCoordinates
        {
            get
            {
                var state = Mouse.GetState();
                return new Tuple<int, int>(state.X, state.Y);
            }
        }

        public void Update()
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && previousState.LeftButton != ButtonState.Pressed)
            {
                EventBus.LatestInstance.Broadcast(EventBusSignal.MouseClicked, null);
            }

            this.previousState = mouseState;
        }
    }
}