using System;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

public class CoreGameScene : Scene
{
    private Entity entity;

    public CoreGameScene()
    {
                this.Add(new Entity()
            .Set(new TextLabelComponent("Hi, mom!")));
    }

    override public void Update()
    {
        var index = DateTime.Now.Second % 4;
        //entity.GetIfHas<SpriteComponent>().FrameIndex = index;
        Console.WriteLine(this.MouseCoordinates.ToString());
    }
}