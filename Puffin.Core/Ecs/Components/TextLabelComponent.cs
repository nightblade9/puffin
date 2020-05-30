using System;
using Puffin.Core.Events;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// Adds a text display to an entity.
    /// </summary>
    public class TextLabelComponent : Component
    {
        public string Text { get; set; } = "";
        
        internal int OutlineColour;
        internal int OutlineThickness = 0;

        // If non-zero, word wrapping is ENABLED.
        internal int WordWrapWidth { get; set; } = 0;


        /// <summary>
        /// The alpha (transparency) of this text and its outline (if it has one), ranging from 0 (invisible) to 1 (fully visible).
        /// See <c>IsVisible</c>.
        /// </summary>
        public float Alpha = 1;

        /// <summary>
        /// The filename of the font, relative to the game directory. Changing this immediately updates the font.
        /// </summary>
        public string FontName { 
            get { return this.fontName; }
            set {
                this.fontName = value;
                this.Parent.Scene?.EventBus.Broadcast(EventBusSignal.LabelFontChanged, this);
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
                this.Parent.Scene?.EventBus.Broadcast(EventBusSignal.LabelFontChanged, this);
            }
        }

        /// <summary>The text colour, in the format 0xRRGGBB with hex values for each pair.</summary>
        public int Colour { get; set; } = 0xFFFFFF;

        internal int OffsetX = 0;
        internal int OffsetY = 0;

        private string fontName;
        private int fontSize = 24;

        /// <summary>
        /// Creates a new text label; if not specified, the default font is 24pt OpenSans.
        /// </summary>
        public TextLabelComponent(Entity parent, string text, int offsetX = 0, int offsetY = 0, string fontName = "", int fontSize = 24)
        : base(parent)
        {
            this.Text = text;
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.Parent.Scene?.EventBus.Broadcast(EventBusSignal.LabelFontChanged, this);
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        /// <summary>
        /// Makes this text component wrap, at the specified width (in pixels); it breaks on newlines. Long words should be hyphenated.
        /// </summary>
        public void WordWrap(int wordWrapWidth)
        {
            this.WordWrapWidth = wordWrapWidth;
        }

        /// <summary>
        /// Outline this text, in the specified colour, and thickness. Note that this is not a true outline; Puffin draws copies of the text
        /// in the specified outline colour, displaced by outline thickness.
        /// Specify a thickness of 0 to remove the outline.
        /// </summary>
        public void Outline(int outlineColour, int outlineThickness)
        {
            if (outlineThickness < 0)
            {
                throw new ArgumentException("Outline thickness must be positive.");
            }
            
            this.OutlineColour = outlineColour;
            this.OutlineThickness = outlineThickness;
        }
    }
}