using System;

namespace Puffin.Core.IO
{
    public interface IKeyboardProvider
    {
        void Update();
        bool IsActionDown(Enum action);
    }
}