using System;
using System.IO;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Tiles;

namespace MyGame
{
    public class CoreGameScene : Scene
    {
        private const int MOVE_VELOCITY = 200;
        private DateTime start = DateTime.Now;

        public CoreGameScene()
        {
            var player = new Entity().Colour(0xFFFFFF, 32, 32)
                .Move(100, 100)
                .FourWayMovement(100)
                .Collide(32, 32, true);
            
            this.Add(player);

            var pushable = new Entity().Colour(0x0000AA, 32, 32)
                .Move(50, 50);
            
            pushable.Collide(32, 32, (e, axis) => {
                if (e == player)
                {
                    var vx = axis == "X" ? (int)player.VelocityX * 2 : 0;
                    var vy = axis == "Y" ? (int)player.VelocityY * 2 : 0;
                    pushable.Velocity(vx, vy);
                }
            }).OnUpdate((elapsed) => {
                pushable.VelocityX *= 0.95f;
                pushable.VelocityY *= 0.95f;
            });

            this.Add(pushable);
        }
    }
}