using System;

namespace Puffin.Core.IO
{
    interface IKeyboardProvider
    {
        void Update();
        bool IsActionDown(Enum action);
        void Reset();
    }
}