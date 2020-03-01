using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace MyGame
{
    public class SubScene : Scene
    {
        private DateTime start = DateTime.Now;
        private int updateMs = 500;
        private DateTime latUpdate = DateTime.Now;
        private Entity player;

        override public void Ready()
        {
            this.player = new Entity().Spritesheet("Content/Charspore.png", 64, 64)
                .Move(400, 400)
                .FourWayMovement(100);
            
            this.Add(player);
        }

        override public void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);
            if ((DateTime.Now - this.latUpdate).TotalMilliseconds >= updateMs)
            {
                this.latUpdate = DateTime.Now;
                var sprite = this.player.Get<SpriteComponent>();
                sprite.FrameIndex = (sprite.FrameIndex + 1) % 12;
            }
        }
    }
}