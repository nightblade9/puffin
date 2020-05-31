using Puffin.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puffin.Core.Tweening
{
    internal class TweenManager
    {
        private List<Tween> tweens = new List<Tween>();

        public void Tween(Entity entity, float durationSeconds, Tuple<float, float> startPosition, Tuple<float, float> endPosition, float startAlpha = 1, float endAlpha = 1, Action onTweenComplete = null)
        {
            // Only one tween at a time, sorry mate
            var toRemove = this.tweens.ToArray().Where(t => t.Entity == entity);
            foreach (var tween in toRemove)
            {
                tween.Stop();
            }
            
            this.tweens.Add(new Tween(entity, durationSeconds, startPosition, endPosition, startAlpha, endAlpha, onTweenComplete));
        }

        public void Update(float elapsedSeconds)
        {
            var toRemove = new List<Tween>();

            // Copy to prevent concurrent exceptions
            foreach (var tween in this.tweens.ToArray())
            {
                if (tween.IsRunning)
                {
                    tween.Update(elapsedSeconds);
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