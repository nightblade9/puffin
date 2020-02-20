namespace Puffin.Core.Ecs.Components
{
    public class CameraComponent : Component
    {
        // TODO: user can specify an entity to follow
        public float Zoom { get; set; }
        
        public CameraComponent(Entity parent, float zoom = 1.0f) : base(parent)
        {
            this.Zoom = zoom;
        }
    }
}