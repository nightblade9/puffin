namespace Puffin.Core.Ecs.Systems
{
    public interface ISystem
    {
        void OnUpdate();
        void OnAddEntity(Entity entity);
    }
}