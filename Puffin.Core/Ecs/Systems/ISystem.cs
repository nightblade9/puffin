using System;

namespace Puffin.Core.Ecs.Systems
{
    public interface ISystem
    {
        void OnUpdate(TimeSpan elapsed);
        void OnAddEntity(Entity entity);
    }
}