namespace Puffin.Core.Ecs.Components
{
    public class TextLabelComponent : Component
    {
        public string Text { get; set; } = "";

        public TextLabelComponent(Entity parent, string text)
        : base(parent)
        {
            this.Text = text;
        }
    }
}