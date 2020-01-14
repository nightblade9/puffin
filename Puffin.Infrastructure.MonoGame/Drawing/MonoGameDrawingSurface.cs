using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    public class MonoGameDrawingSurface : IDrawingSurface
    {
        private IList<Entity> entities = new List<Entity>();
        private IDictionary<Entity, MonoGameSprite> entitySprites = new Dictionary<Entity, MonoGameSprite>();
        
        // TODO: maybe content pipeline is a good thing, amirite? If so, use LoadContent to load sprites
        private GraphicsDevice graphics;
        private SpriteBatch spriteBatch;

        public MonoGameDrawingSurface(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
        }

        public void AddEntity(Entity entity)
        {
            this.entities.Add(entity);
            var sprite = entity.GetIfHas<SpriteComponent>();
            
            if (sprite != null)
            {
                var texture = this.LoadImage(sprite.FileName);
                var monoGameSprite = new MonoGameSprite(entity, texture);
                entitySprites[entity] = monoGameSprite;
            }
        }

        public void DrawAll()
        {
            this.graphics.Clear(Color.DarkSlateGray);
            this.spriteBatch.Begin();

            foreach (var entity in this.entities)
            {
                var monoGameSprite = entitySprites[entity];
                this.spriteBatch.Draw(monoGameSprite.Texture, monoGameSprite.Position, Color.White);
            }
            
            this.spriteBatch.End();
        }

        private Texture2D LoadImage(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var texture = Texture2D.FromStream(this.graphics, stream);
                return texture;
            }
        }
    }
}