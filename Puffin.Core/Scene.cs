using System;
using System.Collections.Generic;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;

namespace Puffin.Core
{     
    public class Scene : IDisposable
    {
        private ISystem[] systems = new ISystem[0];
        private List<Entity> entities = new List<Entity>();

        public void Initialize(params ISystem[] systems)
        {
            this.systems = systems;

            // If called after AddEntity, add entities we know about
            foreach (var entity in this.entities)
            {
                foreach (var system in this.systems)
                {
                    system.OnAddEntity(entity);
                }
            }
        }

        public void Add(Entity entity)
        {
            this.entities.Add(entity);
            
            // if initialized, notify systems
            if (this.systems.Length > 0)
            {
                foreach (var system in this.systems)
                {
                    system.OnAddEntity(entity);
                }
            }
        }

        public void OnUpdate()
        {
            foreach (var system in this.systems)
            {
                system.OnUpdate();
            }
        }

        public void Dispose()
        {
            if (EventBus.LatestInstance != null)
            {
                EventBus.LatestInstance.Dispose();
            }
            
            foreach (var entity in this.entities)
            {
                entity.Dispose();
            }
        }
    }
}
