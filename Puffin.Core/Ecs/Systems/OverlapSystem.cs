using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    public class OverlapSystem : ISystem
    {
        private IList<Entity> entities = new List<Entity>();

        public void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<OverlapComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void OnUpdate(TimeSpan elapsed)
        {
            foreach (var me in this.entities)
            {
                foreach (var target in this.entities)
                {
                    if (me != target)
                    {
                        var myOverlap = me.GetIfHas<OverlapComponent>();
                        var targetOverlap = target.GetIfHas<OverlapComponent>();

                        // Case 1: is me newly-overlapping target?
                        if (myOverlap.IsOverlapping(target) && !myOverlap.WasOverlapping(target))
                        {
                            myOverlap.StartedOverlapping(target);
                        }
                        // Case 2: is me no longer overlapping target?
                        else if (myOverlap.WasOverlapping(target) && !myOverlap.IsOverlapping(target))
                        {
                            myOverlap.StoppedOverlapping(target);
                        }
                    }
                }
            }
        }
    }
}