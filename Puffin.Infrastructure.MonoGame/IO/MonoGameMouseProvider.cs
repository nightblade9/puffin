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

        // Mouse coordinates, taking into account the camera.
        public Tuple<int, int> MouseCoordinates
        {
            get
            {
                var camera = MonoGameDrawingSurface.LatestInstance.GetActiveCamera();
                if (camera != null)
                {
                    var state = Mouse.GetState();
                    var coordinates = CoordinateSpaces.ScreenToWorld(new Vector2(state.X, state.Y), camera.InverseMatrix);
                    return new Tuple<int, int>((int)coordinates.X, (int)coordinates.Y);
                }
                else
                {
                    return this.UiMouseCoordinates;
                }
            }
        }

        // Mouse coordinates, for UI entities (ignores camera).
        public Tuple<int, int> UiMouseCoordinates
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

            if (mouseState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
            {
                this.eventBus.Broadcast(EventBusSignal.MouseClicked);
            }
            else if (mouseState.LeftButton == ButtonState.Released && previousState.LeftButton == ButtonState.Pressed)
            {
                this.eventBus.Broadcast(EventBusSignal.MouseReleased);
            }

            this.previousState = mouseState;
        }

        public void Reset()
        {
            this.previousState = new MouseState();
        }
    }
}