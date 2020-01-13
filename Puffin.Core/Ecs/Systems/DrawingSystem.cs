using System.Collections.Generic;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    public class DrawingSystem : ISystem
    {
        private IDrawingSurface drawingSurface;
        private IList<Entity> entities = new List<Entity>();
        
        public DrawingSystem(IDrawingSurface drawingSurface)
        {
            this.drawingSurface = drawingSurface;
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<SpriteComponent>() != null)
            {
                this.entities.Add(entity);
                this.drawingSurface.AddEntity(entity);
            }
        }

        public void OnUpdate()
        {
            this.drawingSurface.DrawAll();
        }
    }
}