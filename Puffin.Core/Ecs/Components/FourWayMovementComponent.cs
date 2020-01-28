using System;
using Puffin.Core.IO;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a component which makes the entity move in four directions in response to the keyboard.
    /// By default, this responds to the WASD and arrow keys; you can change these bindings
    /// by changing/adding more bindings in your PuffinGame instance.
    /// </summary>
    public class FourWayMovementComponent : KeyboardComponent
    {
        /// <summary>
        /// How fast the entity should move, in pixels per second, when moving in one axis.
        /// </summary>
        public int Speed { get; set; }

        public FourWayMovementComponent(Entity entity, int speed) : base(entity)
        {
            this.Speed = speed;
        }

        internal void OnUpdate(TimeSpan elapsed)
        {
            var vx = 0;
            var vy = 0;

            if (this.IsActionDown(PuffinAction.Up))
            {
                vy = -1;
            } else if (this.IsActionDown(PuffinAction.Down))
            {
                vy = 1;
            }

            if (this.IsActionDown(PuffinAction.Left))
            {
                vx = -1;
            } else if (this.IsActionDown(PuffinAction.Right))
            {
                vx = 1;
            }

            var elapsedSeconds = (float)elapsed.TotalSeconds;

            this.Parent.IntendedMoveDeltaX += (vx * this.Speed * elapsedSeconds);
            this.Parent.IntendedMoveDeltaY += (vy * this.Speed * elapsedSeconds);
            if (vx != 0 && vy != 0)
            {
                vx = (int)(vx / Math.Sqrt(2));
                vy = (int)(vy / Math.Sqrt(2));
            }
        }
    }
}