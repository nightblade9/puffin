using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Drawing
{
    public interface IDrawingSurface
    {
        void Draw(SpriteComponent sprite);
    }
}