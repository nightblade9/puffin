using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.Infrastructure.MonoGame.Drawing;
using System;
using System.Linq;

namespace Puffin.Infrastructure.MonoGame.IO
{
    /// <summary>
    /// An implementation of IMouseProvider using MonoGame's `Mouse` class.
    /// </summary>
    class MonoGameMouseProvider : IMouseProvider
    {
        private MouseState previousState;
        private readonly EventBus eventBus;

        public MonoGameMouseProvider(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public Tuple<int, int> MouseCoordinates
        {
            get
            {
                var state = Mouse.GetState();
                var camera = MonoGameDrawingSurface.LatestInstance.GetActiveCamera();
                if (camera != null)
                {
                    var coordinates = CoordinateSpaces.ScreenToWorld(new Vector2(state.X, state.Y), camera.InverseMatrix);
                    return new Tuple<int, int>((int)coordinates.X, (int)coordinates.Y);
                }
                else
                {
                    return new Tuple<int, int>(state.X, state.Y);
                }
            }
        }

        public void Update()
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && previousState.LeftButton != ButtonState.Pressed)
            {
                this.eventBus.Broadcast(EventBusSignal.MouseClicked, null);
            }

            this.previousState = mouseState;
        }
    }
}