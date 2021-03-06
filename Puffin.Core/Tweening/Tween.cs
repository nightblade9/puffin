using System;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Tweening
{
    internal class Tween
    {
        public Entity Entity { get; private set; }
        public Tuple<float, float> StartPosition { get; private set; }
        public Tuple<float, float> EndPosition { get; private set; }
        public float StartAlpha { get; private set; }
        public float EndAlpha { get; private set; }
        private readonly Action onTweenComplete;
        public float DurationSeconds { get; private set; }
        internal bool IsRunning = false;
        private float runningForSeconds = 0;

        public Tween(Entity entity, float durationSeconds, Tuple<float, float> startPosition, Tuple<float, float> endPosition, float startAlpha = 1, float endAlpha = 1, Action onTweenComplete = null)
        {
            if (startAlpha < 0 || startAlpha > 1)
            {
                throw new ArgumentException($"startAlpha must be between 0 and 1");
            }
            if (endAlpha < 0 || endAlpha > 1)
            {
                throw new ArgumentException("endAlpha must be between 0 and 1");
            }
            
            this.Entity = entity;
            this.DurationSeconds = durationSeconds;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition; 
            this.StartAlpha = startAlpha;
            this.EndAlpha = endAlpha;
            this.onTweenComplete = onTweenComplete;

            this.Start();
        }

        // Applied every frame, assumption is linear tween
        internal float Dx { get { return this.EndPosition.Item1 - this.StartPosition.Item1; } }
        internal float Dy { get { return this.EndPosition.Item2 - this.StartPosition.Item2; } }
        internal float DAlpha { get { return (this.EndAlpha - this.StartAlpha) / this.DurationSeconds; } }

        public void Start()
        {
            if (!this.IsRunning)
            {
                this.IsRunning = true;
                this.Entity.X = this.StartPosition.Item1;
                this.Entity.Y = this.StartPosition.Item2;
                var sprite = this.Entity.Get<SpriteComponent>();
                if (sprite != null)
                {
                    sprite.Alpha = this.StartAlpha;
                }
                this.runningForSeconds = 0;
            }
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                this.onTweenComplete?.Invoke();
            }
        }

        public void Update(float elapsedSeconds)
        {
            var moveSeconds = elapsedSeconds;
            // Don't go over-time
            if (this.runningForSeconds + elapsedSeconds >= this.DurationSeconds)
            {
                moveSeconds = this.DurationSeconds - this.runningForSeconds;
                this.runningForSeconds = this.DurationSeconds;
                this.Stop();
            }
            else
            {
                // Original value, not the nerfed value, which might be 0 after clamping
                this.runningForSeconds += elapsedSeconds;
            }

            this.Entity.X += this.Dx * (moveSeconds / this.DurationSeconds);
            this.Entity.Y += this.Dy * (moveSeconds / this.DurationSeconds);
            
            var sprite = this.Entity.Get<SpriteComponent>();
            if (sprite != null)
            {
                sprite.Alpha = this.StartAlpha + (this.DAlpha * this.runningForSeconds);
            }

            var text = this.Entity.Get<TextLabelComponent>();
            if (text != null)
            {
                text.Alpha = this.StartAlpha + (this.DAlpha * this.runningForSeconds);
            }
        }
    }
}