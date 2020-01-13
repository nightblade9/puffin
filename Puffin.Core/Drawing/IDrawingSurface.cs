using Puffin.Core.Ecs;

namespace Puffin.Core.Drawing
{
    public interface IDrawingSurface
    {
        void DrawAll();
        void AddEntity(Entity entity);
    }
}