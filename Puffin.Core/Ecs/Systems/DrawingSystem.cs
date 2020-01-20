using System;
using System.Collections.Generic;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    class DrawingSystem : ISystem
    {
        private IDrawingSurface drawingSurface;
        private IList<Entity> entities = new List<Entity>();
        
        public DrawingSystem(IDrawingSurface drawingSurface)
        {
            this.drawingSurface = drawingSurface;
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<SpriteComponent>() != null || entity.GetIfHas<TextLabelComponent>() != null)
            {
                this.entities.Add(entity);
                this.drawingSurface.AddEntity(entity);
            }
        }

        // TODO: do this on Draw instead of Update
        public void OnUpdate(TimeSpan elapsed)
        {
            this.drawingSurface.DrawAll();
        }
    }
}