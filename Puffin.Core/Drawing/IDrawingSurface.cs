using Puffin.Core.Ecs;

namespace Puffin.Core.Drawing
{
    public interface IDrawingSurface
    {
        void AddEntity(Entity entity);
        void DrawAll();
    }
}