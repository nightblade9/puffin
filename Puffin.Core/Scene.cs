using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;

namespace Puffin.Core
{     
    public class Scene
    {
        private ISystem[] systems = new ISystem[0];

        public void Initialize(params ISystem[] systems)
        {
            this.systems = systems;
        }

        public void Add(Entity entity)
        {
            foreach (var system in this.systems)
            {
                system.OnAddEntity(entity);
            }
        }

        public void OnUpdate()
        {
            foreach (var system in this.systems)
            {
                system.OnUpdate();
            }
        }
    }
}
