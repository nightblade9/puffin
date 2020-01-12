using System.Collections.Generic;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    public class SpriteDrawingSystem : ISystem
    {
        private IDrawingSurface drawingSurface;
        private IList<Entity> entities = new List<Entity>();
        
        public SpriteDrawingSystem(IDrawingSurface drawingSurface)
        {
            this.drawingSurface = drawingSurface;
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<SpriteComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void OnUpdate()
        {
            foreach (var entity in this.entities)
            {
                var sprite = entity.GetIfHas<SpriteComponent>();
                drawingSurface.Draw(sprite);
            }
        }
    }
}