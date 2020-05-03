using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Puffin.Core.IO;
using Puffin.Infrastructure.MonoGame.Drawing;
using System;

namespace Puffin.Infrastructure.MonoGame.IO
{
    /// <summary>
    /// An implementation of IMouseProvider using MonoGame's `Mouse` class.
    /// </summary>
    class MonoGameMouseProvider : IMouseProvider
    {
        // Mouse coordinates, taking into account the camera.
        public Tuple<int, int> MouseCoordinates
        {
            get
            {
                var camera = MonoGameDrawingSurface.LatestInstance.GetActiveCamera();
                float maxScale = Math.Max(PuffinGame.LatestInstance.Scale.Item1, PuffinGame.LatestInstance.Scale.Item2);
                if (camera != null)
                {
                    var state = Mouse.GetState();
                    var coordinates = CoordinateSpaces.ScreenToWorld(new Vector2(state.X, state.Y), camera.InverseMatrix / maxScale);
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
                float maxScale = Math.Max(PuffinGame.LatestInstance.Scale.Item1, PuffinGame.LatestInstance.Scale.Item2);
                var state = Mouse.GetState();
                return new Tuple<int, int>((int)(state.X / maxScale), (int)(state.Y / maxScale));
            }
        }

        public bool IsLeftButtonDown => Mouse.GetState().LeftButton == ButtonState.Pressed;
    }
}