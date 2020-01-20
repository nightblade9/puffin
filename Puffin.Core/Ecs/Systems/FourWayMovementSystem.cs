using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    public class FourWayMovementSystem : ISystem
    {
        private IList<Entity> entities = new List<Entity>();

        public void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<FourWayMovementComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void OnUpdate(TimeSpan elapsed)
        {
            foreach (var entity in this.entities)
            {
                entity.GetIfHas<FourWayMovementComponent>().OnUpdate(elapsed);
            }
        }
    }
}