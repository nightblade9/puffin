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

        public virtual void OnAddEntity(Entity entity)
        {
            if (
                entity.GetIfHas<SpriteComponent>() != null ||
                entity.GetIfHas<TextLabelComponent>() != null ||
                entity.GetIfHas<ColourComponent>() != null)
            {
                this.entities.Add(entity);
                this.drawingSurface.AddEntity(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            this.drawingSurface.RemoveEntity(entity);
        }

        public virtual void OnUpdate(TimeSpan elapsed)
        {
            
        }
        
        public virtual void OnDraw(TimeSpan elapsed)
        {
            this.drawingSurface.DrawAll();
        }
    }
}