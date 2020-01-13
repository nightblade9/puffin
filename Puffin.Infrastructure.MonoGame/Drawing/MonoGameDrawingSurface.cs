using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Puffin.Infrastructure.MonoGame
{
    public class MonoGameDrawingSurface : IDrawingSurface
    {
        // Optimization: keep one sprite per filename, not per instance. If players ever write code
        // that modifies a sprite, it will affect all the other entities that use that sprite file.
        private Dictionary<string, Texture2D> fileNameToTextureMap = new Dictionary<string, Texture2D>();
        private IList<Entity> entities = new List<Entity>();
        
        // TODO: maybe content pipeline is a good thing, amirite? If so, use LoadContent to load sprites
        

        public void DrawAll()
        {
            GraphicsDevice.Clear(Color.DarkSalmon);
            this.spriteBatch.Begin();

            foreach (var sprite in this.fileNameToTextureMap.Values)
            {
                // TODO: draw at the appropriate coordinates
                this.spriteBatch.Draw(sprite, Vector2.ZERO);
            }
            
            this.spriteBatch.End();
        }

        private var LoadImage(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Read))
            {
                var texture = Texture2D.FromStream(Game.GraphicsDevice, stream);
                return texture;
            }
        }
    }
}