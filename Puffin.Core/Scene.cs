using System;
using System.Collections.Generic;
using Puffin.Core.Drawing;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core
{     
    public class Scene
    {
        private List<Entity> entities = new List<Entity>();

        public void Add(Entity entity)
        {
            this.entities.Add(entity);
        }

        // TODO: goes into the rendering system
        public void OnUpdate(IDrawingSurface drawingSurface)
        {
            foreach (var entity in this.entities)
            {
                SpriteComponent sprite = entity.GetIfHas<SpriteComponent>();
                if (sprite != null)
                {
                    drawingSurface.Draw(sprite);
                }
            }
        }
    }
}
