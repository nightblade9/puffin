using System;

namespace Puffin.Core.IO
{
    interface IMouseProvider
    {
        void Update();
        Tuple<int, int> MouseCoordinates { get; }
        Tuple<int, int> UiMouseCoordinates { get; }
        void Reset();
    }
}