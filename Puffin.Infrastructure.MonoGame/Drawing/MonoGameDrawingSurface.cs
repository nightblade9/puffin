using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Puffin.Infrastructure.MonoGame
{
    public class MonoGameDrawingSurface : Game, IDrawingSurface
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Forceably singleton, we don't ever want more than one (because of SpriteBatch).
        private static var createdFirstInstance = false;

        // Optimization: keep one sprite per filename, not per instance. If players ever write code
        // that modifies a sprite, it will affect all the other entities that use that sprite file.
        private Dictionary<string, Texture2D> fileNameToTextureMap = new Dictionary<string, Texture2D>();
        private IList<Entity> entities = new List<Entity>();
        
        // TODO: maybe content pipeline is a good thing, amirite? If so, use LoadContent to load sprites
        

        public MonoGameDrawingSurface()
        {
            if (createdFirstInstance)
            {
                throw new InvalidOperationException("MonoGameDrawingSurface can't have more than one instance (this is the second one).");
            }

            createdFirstInstance = true;
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void AddEntity(Entity entity)
        {
            var sprite = entity.GetIfHas<SpriteComponent>();
            if (sprite != null)
            {
                entities.Add(entity);
                if (this.canLoadTextures)
                {
                    this.fileNameToTextureMap[sprite.FileName] = this.LoadImage(sprite.Filename);
                }
            }
        }

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

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawingSurface  = new MonoGameDrawingSurface(spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            base.Draw(gameTime);
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