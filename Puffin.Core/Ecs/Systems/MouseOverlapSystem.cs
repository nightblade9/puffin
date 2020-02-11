using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;

namespace Puffin.Core.Ecs.Systems
{
    /// <summary>
    /// Checks for mouse/entity overlaps for entities with an overlap component.
    /// </summary>
    class MouseOverlapSystem : ISystem
    {
        private IList<Entity> entities = new List<Entity>();

        private IMouseProvider mouseProvider;
        private IList<Entity> mouseCurrentlyOverlapping = new List<Entity>();

        public MouseOverlapSystem(IMouseProvider mouseProvider)
        {
            this.mouseProvider = mouseProvider;
        }

        public void OnAddEntity(Entity entity)
        {
            if (entity.Get<OverlapComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
        }

        public void OnUpdate(TimeSpan elapsed)
        {
            foreach (var entity in this.entities)
            {
                if (IsMouseOverlapping(entity)  && !mouseCurrentlyOverlapping.Contains(entity))
                {
                    mouseCurrentlyOverlapping.Add(entity);
                    entity.Get<OverlapComponent>().OnMouseEnter?.Invoke();
                }
                else if (!IsMouseOverlapping(entity) && mouseCurrentlyOverlapping.Contains(entity))
                {
                    mouseCurrentlyOverlapping.Remove(entity);
                    entity.Get<OverlapComponent>().OnMouseExit?.Invoke();
                }
            }
        }

        public bool IsMouseOverlapping(Entity e)
        {
            var overlap = e.Get<OverlapComponent>();
            var mouseCoordinates = this.mouseProvider.MouseCoordinates;
            var mouseX = mouseCoordinates.Item1;
            var mouseY = mouseCoordinates.Item2;

            return 
                mouseX >= e.X && mouseX <= e.X + overlap.Size.Item1 &&
                mouseY >= e.Y && mouseY <= e.Y + overlap.Size.Item2;
        }
    }
}