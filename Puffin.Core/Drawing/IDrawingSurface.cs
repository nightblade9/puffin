using Puffin.Core.Ecs;

namespace Puffin.Core.Drawing
{
    /// <summary>
    /// An internal interface that wraps around the thing we actually draw on (eg. SpriteBatch).
    /// </summary>
    public interface IDrawingSurface
    {
        void DrawAll();
        void AddEntity(Entity entity);
    }
}