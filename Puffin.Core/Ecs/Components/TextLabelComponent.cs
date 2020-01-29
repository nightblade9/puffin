using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a text display to an entity.
    /// </summary>
    public class TextLabelComponent : Component
    {
        public string Text { get; set; } = "";

        /// <summary>
        /// The filename of the font, relative to the game directory. Changing this immediately updates the font.
        /// </summary>
        public string FontName { 
            get { return this.fontName; }
            set {
                this.fontName = value;
                EventBus.LatestInstance.Broadcast(EventBusSignal.LabelFontChanged, this);
            }
        }

        /// <summary>
        /// The font size of the font. Changing this immediately updates the font.
        /// </summary>
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

        /// <summary>
        /// Creates a new text label; if not specified, the default font is 24pt OpenSans.
        /// </summary>
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