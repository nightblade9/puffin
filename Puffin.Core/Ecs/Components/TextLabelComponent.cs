using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A text-label component: a label with some printable text.
    /// </summary>
    public class TextLabelComponent : Component
    {
        public string Text { get; set; } = "";

        public string FontName { 
            get { return this.fontName; }
            set {
                this.fontName = value;
                EventBus.LatestInstance.Broadcast(EventBusSignal.LabelFontChanged, this);
            }
        }

        public int FontSize { 
            get { return this.fontSize; }
            set {
                if (value <= 0)
                {
                    throw new ArgumentException("Font size must be positive");
                }
                this.fontSize = value;
                EventBus.LatestInstance.Broadcast(EventBusSignal.LabelFontChanged, this);
            }
        }

        private string fontName = "OpenSans";
        private int fontSize = 24;

        public TextLabelComponent(Entity parent, string text, string fontName = "OpenSans", int fontSize = 24)
        : base(parent)
        {
            this.Text = text;
            this.FontName = fontName;
            this.FontSize = fontSize;
            EventBus.LatestInstance.Broadcast(EventBusSignal.LabelFontChanged, this);
        }
    }
}