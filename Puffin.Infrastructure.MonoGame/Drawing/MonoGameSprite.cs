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

        public MonoGameSprite(Entity parent, Texture2D texture)
        {
            this.Texture = texture;
            parent.AddPositionChangeCallback((x, y) => this.Position = new Vector2(x, y));
            // Get initial position set correctly
            this.Position = new Vector2(parent.X, parent.Y);
        }
    }
}