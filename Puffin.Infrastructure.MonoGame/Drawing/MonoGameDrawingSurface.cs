using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    /// <summary>
    /// A drawing surface for MonoGame (a wrapper around SpriteBatch).
    /// </summary>
    internal class MonoGameDrawingSurface : IDrawingSurface
    {
        private readonly SpriteFont defaultFont;

        private IList<Entity> entities = new List<Entity>();
        private IDictionary<Entity, MonoGameSprite> entitySprites = new Dictionary<Entity, MonoGameSprite>();
        
        // TODO: maybe content pipeline is a good thing, amirite? If so, use LoadContent to load sprites
        private GraphicsDevice graphics;
        private SpriteBatch spriteBatch;

        public MonoGameDrawingSurface(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont defaultFont)
        {
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.defaultFont = defaultFont;
        }

        public void AddEntity(Entity entity)
        {
            var sprite = entity.GetIfHas<SpriteComponent>();
            
            if (sprite != null)
            {
                var texture = this.LoadImage(sprite.FileName);
                var monoGameSprite = new MonoGameSprite(entity, texture);
                entitySprites[entity] = monoGameSprite;
                this.entities.Add(entity);
            }
            else if (entity.GetIfHas<TextLabelComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void DrawAll()
        {
            this.graphics.Clear(Color.DarkSlateGray);
            this.spriteBatch.Begin();

            foreach (var entity in this.entities)
            {
                // TODO: iterating over entitySprites.Values might be faster. Profile and test.
                if (entitySprites.ContainsKey(entity))
                {
                    var monoGameSprite = entitySprites[entity];
                    this.spriteBatch.Draw(monoGameSprite.Texture, new Vector2(entity.X, entity.Y), monoGameSprite.Region, Color.White);
                }

                var text = entity.GetIfHas<TextLabelComponent>();
                if (text != null)
                {
                    this.spriteBatch.DrawString(defaultFont, text.Text, new Vector2(entity.X, entity.Y), Color.White);
                }
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