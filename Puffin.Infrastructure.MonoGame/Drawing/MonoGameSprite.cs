using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    internal class MonoGameSprite : IDisposable
    {
        public Texture2D Texture { get; private set; }
        public Rectangle Region { get; private set; }

        public MonoGameSprite(SpriteComponent sprite, Texture2D texture)
        {
            this.Texture = texture;

            // if it's a spritesheet, note the rectangle.
            if (sprite.FrameWidth > 0 && sprite.FrameHeight > 0)
            {
                // Spritesheet
                this.Region = new Rectangle(sprite.FrameIndex * sprite.FrameWidth, 0, sprite.FrameWidth, sprite.FrameHeight);
                EventBus.LatestInstance.Subscribe(EventBusSignal.SpriteSheetFrameIndexChanged, (s) =>
                {
                    if (s == sprite)
                    {
                        var numColumns = texture.Width / sprite.FrameWidth;
                        var numRows = texture.Height / sprite.FrameHeight;
                        var xIndex = sprite.FrameIndex % numColumns;
                        var yIndex = sprite.FrameIndex / numColumns;
                        this.Region = new Rectangle(xIndex * sprite.FrameWidth, yIndex * sprite.FrameHeight, sprite.FrameWidth, sprite.FrameHeight);
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