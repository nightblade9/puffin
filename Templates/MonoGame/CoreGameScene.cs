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
    private Entity bird;

    public CoreGameScene()
    {
        this.textLabel = new Entity().Label("No keys held down").Move(300, 170);
        this.Add(textLabel);

        /*
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
        );*/

        this.bird = new Entity().Sprite("Bird.png").Keyboard().Move(300, 200);
        this.Add(this.bird);
    }

    override public void Update()
    {
        // var index = DateTime.Now.Second % 4;
        // tilemapEntity.GetIfHas<SpriteComponent>().FrameIndex = index;
        
        var label = this.textLabel.GetIfHas<TextLabelComponent>();
        var keyboard = this.bird.GetIfHas<KeyboardComponent>();
        var text = "";

        if (keyboard.IsActionDown(Puffin.Core.IO.PuffinAction.Up))
        {
            text += "Up ";
        }
        if (keyboard.IsActionDown(Puffin.Core.IO.PuffinAction.Down))
        {
            text += "Down ";
        }
        if (keyboard.IsActionDown(Puffin.Core.IO.PuffinAction.Left))
        {
            text += "Left ";
        }
        if (keyboard.IsActionDown(Puffin.Core.IO.PuffinAction.Right))
        {
            text += "Right ";
        }

        label.Text = text;

    }
}