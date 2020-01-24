namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A text-label component: a label with some printable text.
    /// </summary>
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