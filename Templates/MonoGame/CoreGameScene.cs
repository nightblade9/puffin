using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

public class CoreGameScene : Scene
{
    public CoreGameScene()
    {
        this.Add(new Entity().Set(new SpriteComponent("Bird.png")));
    }
}