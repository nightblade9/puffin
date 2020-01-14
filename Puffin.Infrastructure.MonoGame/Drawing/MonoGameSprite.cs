using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    public class MonoGameSprite : Component
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Rectangle Region { get; private set; }

        public MonoGameSprite(Entity parent, Texture2D texture)
        {
            this.Texture = texture;
            parent.AddPositionChangeCallback((x, y) => this.Position = new Vector2(x, y));
            // Get initial position set correctly
            this.Position = new Vector2(parent.X, parent.Y);

            // if it's a spritesheet, note the rectangle.
            var sprite = parent.GetIfHas<SpriteComponent>();
            if (sprite.FrameWidth > 0 && sprite.FrameHeight > 0)
            {
                this.Region = new Rectangle(0, 0, sprite.FrameWidth, sprite.FrameHeight);
            }
            else if (sprite.FrameWidth == 0 && sprite.FrameHeight == 0)
            {
                // Regular image
                this.Region = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            else
            {
                throw new InvalidOperationException($"Frame width/height must be positive, they are currently {sprite.FrameWidth}, {sprite.FrameHeight}");
            }
        }
    }
}