using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Puffin.Infrastructure.MonoGame
{
    public class MonoGameDrawingSurface : IDrawingSurface
    {
        // Optimization: keep one sprite per filename, not per instance. If players ever write code
        // that modifies a sprite, it will affect all the other entities that use that sprite file.
        private Dictionary<string, Texture2D> fileNameToTextureMap = new Dictionary<string, Texture2D>();
        private IList<Entity> entities = new List<Entity>();
        
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
                this.fileNameToTextureMap[sprite.FileName] = this.LoadImage(sprite.FileName);
            }
        }

        public void DrawAll()
        {
            this.graphics.Clear(Color.DarkSlateGray);
            this.spriteBatch.Begin();

            foreach (var sprite in this.fileNameToTextureMap.Values)
            {
                // TODO: draw at the appropriate coordinates
                this.spriteBatch.Draw(sprite, new Vector2(0, 0), Color.White);
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