using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    class OverlapSystem : ISystem
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
                        if (IsOverlapping(me, target) && !WasOverlapping(myOverlap, target))
                        {
                            StartedOverlapping(myOverlap, target);
                        }
                        // Case 2: is me no longer overlapping target?
                        else if (WasOverlapping(myOverlap, target) && !IsOverlapping(me, target))
                        {
                            StoppedOverlapping(myOverlap, target);
                        }
                    }
                }
            }
        }
        
        public static bool WasOverlapping(OverlapComponent me, Entity target)
        {
            return me.CurrentlyOverlapping.Contains(target);
        }

        public static bool IsOverlapping(Entity me, Entity target)
        {
            var overlap = me.GetIfHas<OverlapComponent>();

            var targetOverlap = target.GetIfHas<OverlapComponent>();
            if (targetOverlap == null)
            {
                return false;
            }

            var left = me.X + overlap.Offset.Item1;
            var right = left + overlap.Size.Item1;
            var top = me.Y + overlap.Offset.Item2;
            var bottom = top + overlap.Size.Item2;

            var targetLeft = target.X + targetOverlap.Offset.Item1;
            var targetRight = targetLeft + targetOverlap.Size.Item1;
            var targetTop = target.Y + targetOverlap.Offset.Item2;
            var targetBottom = targetTop + targetOverlap.Size.Item2;

            // Pilfered from MonoGame: https://github.com/MonoGame/MonoGame/blob/6f34eb393aa0ac005888d74c5c4c6ab5615fdc8c/MonoGame.Framework/Rectangle.cs#L398
            return targetLeft < right &&
                left < targetRight &&
                targetTop < bottom &&
                top < targetBottom;
        }

        public static void StartedOverlapping(OverlapComponent me, Entity target)
        {
            if (!me.CurrentlyOverlapping.Contains(target))
            {
                me.CurrentlyOverlapping.Add(target);

                if (me.OnStartOverlap != null)
                {
                    me.OnStartOverlap.Invoke(target);
                }
            }
        }

        public static void StoppedOverlapping(OverlapComponent overlap, Entity target)
        {
            if (overlap.CurrentlyOverlapping.Contains(target))
            {
                overlap.CurrentlyOverlapping.Remove(target);

                if (overlap.OnStopOverlap != null)
                {
                    overlap.OnStopOverlap.Invoke(target);
                }
            }
        }
    }
}