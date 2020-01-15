namespace Puffin.Core.Ecs.Components
{
    public class TextLabelComponent : Component
    {
        public string Text { get; set; } = "";

        public TextLabelComponent(string text)
        {
            this.Text = text;
        }
    }
}