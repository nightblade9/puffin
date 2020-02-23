using System;
using Puffin.Core.Ecs;

namespace Puffin.Core.Tweens
{
    internal class Tween
    {
        public Entity Entity { get; private set; }
        public Tuple<float, float> StartPosition { get; private set; }
        public Tuple<float, float> EndPosition { get; private set; }
        public float DurationSeconds { get; private set; }
        internal bool IsRunning = false;
        private float runningForSeconds = 0;

        public Tween(Entity entity, Tuple<float, float> startPosition, Tuple<float, float> endPosition, float durationSeconds)
        {
            this.Entity = entity;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition; 
            this.DurationSeconds = durationSeconds;

            this.Start();
        }

        // Applied every frame, assumption is linear tween
        public float Dx { get { return this.EndPosition.Item1 - this.StartPosition.Item1; } }
        public float Dy { get { return this.EndPosition.Item2 - this.StartPosition.Item2; }}

        public void Start()
        {
            this.IsRunning = true;
            this.Entity.X = this.StartPosition.Item1;
            this.Entity.Y = this.StartPosition.Item2;
            this.runningForSeconds = 0;
        }

        public void Stop()
        {
            this.IsRunning = false;
        }

        public void Update(int elapsedMilliseconds)
        {
            float elapsedSeconds = elapsedMilliseconds / 1000f;
            // Don't go over-time
            if (this.runningForSeconds + elapsedSeconds > this.DurationSeconds)
            {
                elapsedSeconds = (int)(this.DurationSeconds - this.runningForSeconds);
            }

            this.Entity.X += this.Dx * elapsedSeconds;
            this.Entity.Y += this.Dy * elapsedSeconds;
            // Original value, not the nerfed value, which might be 0
            this.runningForSeconds += (elapsedMilliseconds / 1000f);

            if (this.runningForSeconds >= this.DurationSeconds)
            {
                this.Stop();
            }
        }
    }
}