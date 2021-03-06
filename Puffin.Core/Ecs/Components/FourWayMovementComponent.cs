using System;
using Puffin.Core.IO;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a component which makes the entity move in four directions in response to the keyboard.
    /// By default, responds to the <c>WASD</c> and arrow keys; you can change these bindings
    /// by changing/adding more bindings in your <c>PuffinGame</c> subclass.
    /// Note that setting this component overrides an entity's velocity.
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

        internal void OnUpdate()
        {
            var vx = 0;
            var vy = 0;

            if (this.Parent.Scene.IsActionDown(PuffinAction.Up))
            {
                vy = -1;
            } else if (this.Parent.Scene.IsActionDown(PuffinAction.Down))
            {
                vy = 1;
            }

            if (this.Parent.Scene.IsActionDown(PuffinAction.Left))
            {
                vx = -1;
            } else if (this.Parent.Scene.IsActionDown(PuffinAction.Right))
            {
                vx = 1;
            }

            this.Parent.VelocityX = vx * this.Speed;
            this.Parent.VelocityY = vy * this.Speed;
        }
    }
}