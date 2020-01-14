using Microsoft.Xna.Framework.Graphics;
using Puffin.Core.Ecs.Components;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    public class MonoGameSprite : Component
    {
        public Texture2D Texture { get; private set; }

        public MonoGameSprite(Texture2D texture)
        {
            this.Texture = texture;
        }
    }
}