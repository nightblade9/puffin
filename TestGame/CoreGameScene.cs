using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.IO;

namespace MyGame
{
    public class CoreGameScene : Scene
    {
        override public void Ready()
        {
            var player = new Entity().Colour(0xFF0000, 32, 32)
                .Move(100, 100)
                .FourWayMovement(100)
                .Collide(32, 32, true);
            
            this.Add(player);

            this.OnActionPressed = (val) => {
                var action = (PuffinAction)val;
                if (action == PuffinAction.Down)
                {
                    this.ShowSubScene(new SubScene(this));
                    Console.WriteLine("SHOW");
                }
            };
        }
    }
}