using Puffin.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puffin.Core.Tweening
{
    internal class TweenManager
    {
        internal static TweenManager LatestInstance { get; set; }
        private List<Tween> tweens = new List<Tween>();

        public TweenManager()
        {
            TweenManager.LatestInstance = this;
        }

        public void TweenPosition(Entity entity, Tuple<float, float> startPosition, Tuple<float, float> endPosition, float durationSeconds, Action onTweenComplete = null)
        {
            // Only one tween at a time, sorry mate
            var toRemove = this.tweens.Where(t => t.Entity == entity);
            foreach (var tween in toRemove)
            {
                tween.Stop();
            }
            
            this.tweens.Add(new Tween(entity, startPosition, endPosition, durationSeconds, onTweenComplete));
        }

        public void Update(float elapsedSeconds)
        {
            var toRemove = new List<Tween>();

            foreach (var tween in this.tweens)
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