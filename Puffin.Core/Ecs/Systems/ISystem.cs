using System;

namespace Puffin.Core.Ecs.Systems
{
    interface ISystem
    {
        void OnUpdate(TimeSpan elapsed);
        void OnAddEntity(Entity entity);
        void OnRemoveEntity(Entity entity);
    }
}