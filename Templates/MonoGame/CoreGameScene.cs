using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

public class CoreGameScene : Scene
{
    private Entity tilemapEntity;
    private Entity textLabel;

    public CoreGameScene()
    {
        this.tilemapEntity = new Entity()
            .Set(new SpriteComponent("tilemap.png", 32, 32))
            .Move(300, 200);
        
        this.Add(tilemapEntity);

        this.textLabel = new Entity().Set(new TextLabelComponent("(0, 0)"));

        this.Add(textLabel);
    }

    override public void Update()
    {
        var index = DateTime.Now.Second % 4;
        tilemapEntity.GetIfHas<SpriteComponent>().FrameIndex = index;
        
        this.textLabel.GetIfHas<TextLabelComponent>().Text = $"Mouse: {this.MouseCoordinates.ToString()}";
    }
}