using System;

namespace Puffin.Core.IO
{
    public interface IMouseProvider
    {
        void Update();
        Tuple<int, int> MouseCoordinates { get; }
    }
}