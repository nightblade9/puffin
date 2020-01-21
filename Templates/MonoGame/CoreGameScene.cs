using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace MyGame
{
    public class CoreGameScene : Scene
    {
        private Random random = new Random();
        public CoreGameScene()
        {
            for (var i = 0; i < 2500; i++)
            {
                this.Add(new Entity().Spritesheet("Charspore.png", 64, 64).Move(random.Next(960), random.Next(540)));
            }
        }
    }
}