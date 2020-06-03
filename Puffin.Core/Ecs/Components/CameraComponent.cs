namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A component that allows you to zoom in or out. Note that entities that are flagged as <c>IsUiElement</c>
    /// are not affected by zoom.
    /// </summary>
    public class CameraComponent : Component
    {
        /// <summary>
        /// Sets the zoom of this camera, which displays all non-UI entities at the specified zoom.
        /// </summary>
        public float Zoom { get; set; }
        
        public CameraComponent(Entity parent, float zoom = 1.0f) : base(parent)
        {
            this.Zoom = zoom;
        }
    }
}