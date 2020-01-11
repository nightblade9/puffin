namespace Puffin.Core.Ecs.Components
{
    public class SpriteComponent : Component
    {
        private string fileName = "";

        public SpriteComponent(string fileName)
        {
            this.fileName = fileName;
        }
    }
}