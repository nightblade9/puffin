using System;
using Microsoft.Xna.Framework.Input;
using Puffin.Core.IO;

namespace Puffin.Infrastructure.MonoGame.IO
{
    public class MonoGameMouseProvider : IMouseProvider
    {
        public Tuple<int, int> MouseCoordinates {
            get {
                var state = Mouse.GetState();
                return new Tuple<int, int>(state.X, state.Y);
            }
        }
    }
}