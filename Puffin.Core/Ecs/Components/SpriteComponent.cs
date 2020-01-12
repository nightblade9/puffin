namespace Puffin.Core.Ecs.Components
{
    public class SpriteComponent : Component
    {
        public string FileName { get; private set; }

        public SpriteComponent(string fileName)
        {
            this.FileName = fileName;
        }
    }
}