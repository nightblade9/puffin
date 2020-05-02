using System;
using Puffin.Core.Ecs;

namespace Puffin.Core.IO
{
    internal interface IAudioPlayer
    {
        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
        void OnUpdate();
    }
}