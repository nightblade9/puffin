using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;

namespace MyGame
{
    public class CoreGameScene : Scene
    {
        public CoreGameScene() : base()
        {
            
        }
        override public void Ready()
        {
            var e = new Entity().Label("HIIII");
            e.Get<TextLabelComponent>().FontSize = 72;
            this.Add(e);
            
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