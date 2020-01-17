using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Infrastructure.MonoGame.IO;
using Puffin.UI.Controls;

public class CoreGameScene : Scene
{
    private Entity tilemapEntity;
    private Entity textLabel;

    public CoreGameScene()
    {
        this.textLabel = new Entity().Label("(0, 0)");
        this.Add(textLabel);

        this.tilemapEntity = new Entity()
            .Spritesheet("tilemap.png", 32, 32)
            .Move(300, 200)
            .Mouse(() => {
                tilemapEntity.GetIfHas<SpriteComponent>().FrameIndex++;
                tilemapEntity.GetIfHas<SpriteComponent>().FrameIndex %= 4;
            }, 32, 32);
        
        this.Add(tilemapEntity);

        //this.OnMouseClick = () => textLabel.GetIfHas<TextLabelComponent>().Text = $"Mouse: {this.MouseCoordinates}";

        this.Add(
            new Button("Click me!",
                () => textLabel.GetIfHas<TextLabelComponent>().Text = "WOW!!")
            .Move(500, 100)
        );
    }

    override public void Update()
    {
        // var index = DateTime.Now.Second % 4;
        // tilemapEntity.GetIfHas<SpriteComponent>().FrameIndex = index;
        
        //this.textLabel.GetIfHas<TextLabelComponent>().Text = $"Mouse: {this.MouseCoordinates.ToString()}";
    }
}