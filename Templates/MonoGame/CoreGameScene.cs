using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace MyGame
{
    public class CoreGameScene : Scene
    {
        private Random random = new Random();
        private Entity player;

        public CoreGameScene()
        {
            player = new Entity().FourWayMovement(100)
                //.Sprite("Content/square-white.png")
                .Overlap(32, 32)
                .Move(300, 300)
                .Label("HI MOM!!!")
                .Audio("test.wav");
            
            player.Set(new ColourComponent(player, 0xFF8800, 64, 64));

            player.Mouse(() => {
                float pitch = (float)(0.5 + (random.NextDouble() % 0.5));
                Console.WriteLine($"Pitch={pitch}");
                //player.GetIfHas<AudioComponent>().Play(pitch);
                //this.Remove(player);

                player.GetIfHas<TextLabelComponent>().FontSize = 72;
            }, 32, 32);

            this.Add(player);
        }

        override public void Update()
        {
            if (this.IsActionDown(CustomAction.Next))
            {
                this.Remove(player);
            }
        }
    }
}