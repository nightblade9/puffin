using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace MyGame
{
    public class CoreGameScene : Scene
    {

        public CoreGameScene()
        {
            var player = new Entity().FourWayMovement(100)
                .Sprite("Content/square-white.png")
                .Overlap(32, 32)
                .Move(300, 300);

            var stove = new Entity().Label("").Sprite("Content/square-red.png").Move(200, 200);
            stove.Overlap(64, 64, -16, -16,
                    (e) => {
                        if (e == player) {
                            stove.GetIfHas<TextLabelComponent>().Text = "Stove";
                        }
                    },
                    (e) => {
                        if (e == player) {
                            stove.GetIfHas<TextLabelComponent>().Text = "";
                        }
                    });

            this.Add(player);
            this.Add(stove);
        }
    }
}