using Puffin.Core.Ecs;
using System;
using System.Collections.Generic;

namespace Puffin.Core.Tweens
{
    /// <summary>
    /// A class that manages tweens.
    /// </summary>
    public class TweenManager
    {
        private List<Tween> tweens = new List<Tween>();

        public void TweenPosition(Entity entity, Tuple<float, float> startPosition, Tuple<float, float> endPosition, float durationSeconds)
        {
            // Only one tween at a time, sorry mate
            this.tweens.RemoveAll(t => t.Entity == entity);
            this.tweens.Add(new Tween(entity, startPosition, endPosition, durationSeconds));
        }

        public void Update(int elapsedMilliseconds)
        {
            var toRemove = new List<Tween>();

            foreach (var tween in this.tweens)
            {
                if (tween.IsRunning)
                {
                    tween.Update(elapsedMilliseconds);
                }
                else
                {
                    toRemove.Add(tween);
                }
            }

            foreach (var tween in toRemove)
            {
                this.tweens.Remove(tween);
            }
        }
    }
}