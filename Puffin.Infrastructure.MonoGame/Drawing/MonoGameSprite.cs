using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    internal class MonoGameSprite : Component, IDisposable
    {
        public Texture2D Texture { get; private set; }
        public Rectangle Region { get; private set; }

        public MonoGameSprite(Entity parent, Texture2D texture)
        : base(parent)
        {
            this.Texture = texture;

            // if it's a spritesheet, note the rectangle.
            var sprite = parent.GetIfHas<SpriteComponent>();
            if (sprite.FrameWidth > 0 && sprite.FrameHeight > 0)
            {
                // Spritesheet
                this.Region = new Rectangle(0, 0, sprite.FrameWidth, sprite.FrameHeight);
                EventBus.LatestInstance.Subscribe(EventBusSignal.SpriteSheetFrameIndexChanged, (s) =>
                {
                    if (s == sprite)
                    {
                        this.Region = new Rectangle(sprite.FrameIndex * sprite.FrameWidth, 0, sprite.FrameWidth, sprite.FrameHeight);
                    }
                });
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

        public void Dispose()
        {
            this.Texture.Dispose();
        }
    }
}