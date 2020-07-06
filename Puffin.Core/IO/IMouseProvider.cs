using System;

namespace Puffin.Core.IO
{
    interface IMouseProvider
    {
        Tuple<int, int> MouseCoordinates { get; }
        Tuple<int, int> UiMouseCoordinates { get; }
        bool IsButtonDown(ClickType clickType);
    }
}