using System;

namespace Puffin.Core.IO
{
    public interface IMouseProvider
    {
        Tuple<int, int> MouseCoordinates { get; }
    }
}