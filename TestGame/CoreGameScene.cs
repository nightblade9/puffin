using System;
using System.IO;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Tiles;

namespace MyGame
{
    public class CoreGameScene : Scene
    {
        public CoreGameScene()
        {
            var tileMap = new TileMap(30, 17, Path.Combine("Content", "dungeon.png"), 32, 32);
            tileMap.Define("Floor", 0, 0);
            tileMap.Define("Wall", 1, 0, true);

            for (var y = 0; y < 17; y++) {
                for (var x = 0; x < 30; x++) {
                    if (x == 0 || y == 0 || x == 29 || y == 16) {
                        tileMap[x, y] = "Wall";
                    } else {
                        tileMap[x, y] = "Floor";
                    }
                }
            }

            this.Add(tileMap);

            this.Add(new Entity().Colour(0xFFFFFF, 32, 32)
                .FourWayMovement(100, true).
                Move(48, 48).Collide(32, 32));

            this.Add(new Entity().Colour(0xFF0000, 128, 64).Move(100, 100).Collide(128, 64));
        }
    }
}