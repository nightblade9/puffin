using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

public class CoreGameScene : Scene
{
    private Entity entity;

    public CoreGameScene()
    {
        this.entity = new Entity()
            .Set(new SpriteComponent("tilemap.png", 32, 32))
            .Move(300, 200);

        this.Add(this.entity);
    }

    override public void Update()
    {
        var index = DateTime.Now.Second % 4;
        entity.GetIfHas<SpriteComponent>().FrameIndex = index;
        Console.WriteLine(this.MouseCoordinates.ToString());
    }
}