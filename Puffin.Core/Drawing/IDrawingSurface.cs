using Puffin.Core.Ecs;

namespace Puffin.Core.Drawing
{
    interface IDrawingSurface
    {
        void DrawAll();
        void AddEntity(Entity entity);
    }
}