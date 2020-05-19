namespace Puffin.Core.Ecs.Components
{
    public class CameraComponent : Component
    {
        // TODO: user can specify an entity to follow

        /// <summary>
        /// Sets the zoom of this camera, which displays all non-UI entities at the specified zoom.
        /// Note that zoom affects tilesets, text/label components, etc.
        /// </summary>
        public float Zoom { get; set; }
        
        public CameraComponent(Entity parent, float zoom = 1.0f) : base(parent)
        {
            this.Zoom = zoom;
        }
    }
}