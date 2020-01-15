using System;
using System.Collections.Generic;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core
{     
    public class Scene : IDisposable
    {
        private IMouseProvider mouseProvider;
        private ISystem[] systems = new ISystem[0];
        private List<Entity> entities = new List<Entity>();

        public Tuple<int, int> MouseCoordinates { get { return this.mouseProvider.MouseCoordinates; }}

        public void Initialize(ISystem[] systems, IMouseProvider mouseProvider)
        {
            this.systems = systems;
            this.mouseProvider = mouseProvider;

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

        /// <summary>
        /// Internal method that calls `Update` on all systems in this scene.
        /// </summary>
        public void OnUpdate()
        {
            foreach (var system in this.systems)
            {
                system.OnUpdate();
            }

            this.Update();
        }

        /// <summary>
        /// A method that's called every time Update is called by the game engine.
        /// Override it to do things "every frame."
        /// </summary>
        public virtual void Update()
        {
            
        }

        public void Dispose()
        {
            if (EventBus.LatestInstance != null)
            {
                EventBus.LatestInstance.Dispose();
            }

            // Reset EventBus.LatestIntance
            new EventBus();
        }
    }
}
