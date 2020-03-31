using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.Drawing;
using Puffin.Core.Tiles;
using SpriteFontPlus;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    /// <summary>
    /// A drawing surface for MonoGame (a wrapper around SpriteBatch).
    /// </summary>
    internal class MonoGameDrawingSurface : IDrawingSurface, IDisposable
    {
        public static MonoGameDrawingSurface LatestInstance { get; private set; }

        private readonly EventBus eventBus;
        private readonly SpriteFont defaultFont;

        private IList<Entity> entities = new List<Entity>();
        private IList<Entity> uiEntities = new List<Entity>();

        internal IList<Entity> cameras = new List<Entity>();

        // TODO: This collection smells. Should we just add these things as components? But that breaks user expectations and serialization.
        private IDictionary<Entity, MonoGameSprite> entitySprites = new Dictionary<Entity, MonoGameSprite>();
        private IDictionary<TileMap, Texture2D> tileMapSprites = new Dictionary<TileMap, Texture2D>();
        private IDictionary<Entity, MonoGameCamera> entityCameras = new Dictionary<Entity, MonoGameCamera>();

        private IDictionary<Entity, SpriteFont> entityFonts = new Dictionary<Entity, SpriteFont>();        
        // "name, size" => font. Cache of all fonts ever seen so far.
        private IDictionary<string, SpriteFont> allFonts = new Dictionary<string, SpriteFont>();
        
        private readonly GraphicsDevice graphics;
        private readonly SpriteBatch spriteBatch;

        private Texture2D backgroundSprite;
        // 1x1 white rectangle, used to draw colour components
        private readonly Texture2D whiteRectangle;

        // Alpha is from a ColourComponent, from 0 (invisible) to 1 (opaque).
        private static Color BgrToRgba(int packedRgb, float alpha = 1)
        {
            // Although we ask for 0xRRGGBB, the value we get, if we pass it directly to MonoGame,
            // renders as 0xBBGGRR. So, convert.
            int red = (packedRgb >> 16) & 0xFF;
            int green = (packedRgb >> 8) & 0xFF;
            int blue = (packedRgb >> 0) & 0xFF;
            return new Color(red, green, blue, (int)(alpha * 255));
        }

        public MonoGameDrawingSurface(EventBus eventBus, GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            MonoGameDrawingSurface.LatestInstance = this;
            this.eventBus = eventBus;

            whiteRectangle = new Texture2D(graphics, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.defaultFont = this.LoadFont("OpenSans", 24);

            this.eventBus.Subscribe(EventBusSignal.LabelFontChanged, (data) =>
            {
                this.LoadFontFor(data as TextLabelComponent);
            });
        }

        public void AddEntity(Entity entity)
        {            
            if (entity.Get<SpriteComponent>() != null)
            {
                this.entities.Add(entity);
                this.AddMonoGameSpriteFor(entity);
            }
            if (entity.Get<TextLabelComponent>() != null)
            {                
                this.LoadFontFor(entity.Get<TextLabelComponent>());
                if (!this.entities.Contains(entity))
                {
                    this.entities.Add(entity);
                }
            }
            if (entity.Get<ColourComponent>() != null && !this.entities.Contains(entity))
            {
                this.entities.Add(entity);
            }
            if (entity.Get<CameraComponent>() != null)
            {
                this.cameras.Add(entity);
                var monoGamecamera = new MonoGameCamera(this.graphics.Viewport);
                this.entityCameras[entity] = monoGamecamera;

                if (!this.entities.Contains(entity))
                {
                    this.entities.Add(entity);
                }
            }
        }

        public void AddUiEntity(Entity entity)
        {
            this.uiEntities.Add(entity);
            this.AddMonoGameSpriteFor(entity);
            if (entity.Get<TextLabelComponent>() != null)
            {                
                this.LoadFontFor(entity.Get<TextLabelComponent>());
            }           
        }

        public void RemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            
            if (this.entitySprites.ContainsKey(entity))
            {
                var monoGameSprite = this.entitySprites[entity];
                monoGameSprite.Dispose();
                this.entitySprites.Remove(entity);
            }
            
            this.cameras.Remove(entity);
            this.entityCameras.Remove(entity);

            this.uiEntities.Remove(entity);
        }

        public void AddTileMap(TileMap tileMap)
        {
            this.tileMapSprites[tileMap] = LoadImage(tileMap.TileImageFile);
            // we never set the width/height on the tilemap sprite that we loaded?
        }

        public void RemoveTileMap(TileMap tileMap)
        {
            this.tileMapSprites[tileMap].Dispose();
            this.tileMapSprites.Remove(tileMap);
        }

        public void DrawAll(int backgroundColour, string backgroundImage = "", bool clearDisplay = true)
        {
            if (clearDisplay)
            {
                this.graphics.Clear(BgrToRgba(backgroundColour));
                if (!string.IsNullOrEmpty(backgroundImage) && this.backgroundSprite == null)
                {
                    this.backgroundSprite = LoadImage(backgroundImage);
                }
                
                if (this.backgroundSprite != null)
                {
                    this.spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    this.spriteBatch.Draw(this.backgroundSprite, Vector2.Zero, Color.White);
                    this.spriteBatch.End();
                }
            }

            var lastActiveCamera = this.cameras.LastOrDefault();
            MonoGameCamera camera = null;
            if (lastActiveCamera != null)
            {
                camera = this.entityCameras[lastActiveCamera];
                var cameraComponent = lastActiveCamera.Get<CameraComponent>();
                // This smells. How do we synch properties?
                camera.Zoom = new Vector2(cameraComponent.Zoom, cameraComponent.Zoom);
            }

            this.spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera?.TransformationMatrix);
            this.DrawTileMaps();
            this.DrawEntities(this.entities);
            this.spriteBatch.End();

            this.spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            this.DrawEntities(this.uiEntities);
            this.spriteBatch.End();

            // TODO: draw things that are UI flag/layer/etc.

            // Last: draw collision shapes
            if (PuffinGame.LatestInstance.ShowCollisionAreas)
            {
                this.spriteBatch.Begin(transformMatrix: camera?.TransformationMatrix);
                foreach (var entity in this.entities)
                {
                    var collider = entity.Get<CollisionComponent>();
                    if (collider != null)
                    {
                        this.spriteBatch.Draw(whiteRectangle, new Rectangle(
                            (int)entity.X + collider.XOffset, (int)entity.Y + collider.YOffset,
                            collider.Width, collider.Height),
                            Color.Red * 0.5f);
                    }
                }
                this.spriteBatch.End();
            }
        }

        public MonoGameCamera GetActiveCamera()
        {
            var lastActiveCamera = this.cameras.LastOrDefault();
            if (lastActiveCamera != null)
            {
                return this.entityCameras[lastActiveCamera];
            }
            return null;
        }

        public void Dispose()
        {
            this.whiteRectangle.Dispose();

            foreach (var sprite in this.entitySprites.Values)
            {
                sprite.Texture.Dispose();
            }

            foreach (var texture in this.tileMapSprites.Values)
            {
                texture.Dispose();
            }
        }

        private void AddMonoGameSpriteFor(Entity entity)
        {
            if (entity.Get<SpriteComponent>() != null)
            {
                var spriteComponent = entity.Get<SpriteComponent>();
                var texture = this.LoadImage(spriteComponent.FileName);
                spriteComponent.Width = texture.Width;
                spriteComponent.Height = texture.Height;
                var monoGameSprite = new MonoGameSprite(this.eventBus, spriteComponent, texture);
                entitySprites[entity] = monoGameSprite;
            }
        }

        private Texture2D LoadImage(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                var texture = Texture2D.FromStream(this.graphics, stream);
                return texture;
            }
        }

        private SpriteFont LoadFont(string fileName, int fontSize)
        {
             var fontBakeResult = TtfFontBaker.Bake(
                File.ReadAllBytes(Path.Combine("Content", "Fonts", $"{fileName}.ttf")), fontSize, 1024, 1024, 
                new[] {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic });

            var font = fontBakeResult.CreateSpriteFont(this.graphics);
            return font;
        }

        private void DrawEntities(IList<Entity> entities)
        {
            foreach (var entity in entities.ToArray())
            {
                MonoGameSprite monoGameSprite = null;
                this.entitySprites.TryGetValue(entity, out monoGameSprite);
                if (monoGameSprite != null && entity.Get<SpriteComponent>().IsVisible)
                {
                    this.spriteBatch.Draw(monoGameSprite.Texture, new Vector2(entity.X, entity.Y), monoGameSprite.Region, Color.White);
                }

                var colour = entity.Get<ColourComponent>();
                if (colour != null)
                {
                    this.spriteBatch.Draw(whiteRectangle, 
                        new Rectangle((int)entity.X + colour.OffsetX, (int)entity.Y + colour.OffsetY, colour.Width, colour.Height),
                        BgrToRgba(colour.Colour, colour.Alpha));
                }

                var text = entity.Get<TextLabelComponent>();
                if (text != null)
                {
                    if (!this.entityFonts.ContainsKey(entity))
                    {
                        this.entityFonts[entity] = this.defaultFont;
                    }

                    var font = this.entityFonts[entity];
                    this.spriteBatch.DrawString(font, text.Text, new Vector2(entity.X + text.OffsetX, entity.Y + text.OffsetY), BgrToRgba(text.Colour));
                }
            }
        }

        private void DrawTileMaps()
        {
            // TODO: render in order of Z from lowest to highest
            // Tilemaps first, I suppose
            foreach (var tileMap in this.tileMapSprites.Keys)
            {
                var mapTexture = this.tileMapSprites[tileMap];
                for (var y = 0; y < tileMap.MapHeight; y++)
                {
                    for (var x = 0; x < tileMap.MapWidth; x++)
                    {
                        var tile = tileMap[x, y];
                        if (tile != null)
                        {
                            var definition = tileMap.GetDefinition(tile);
                            spriteBatch.Draw(
                                mapTexture,
                                new Vector2(tileMap.X + (x * tileMap.TileWidth), tileMap.Y + (y * tileMap.TileHeight)),
                                new Rectangle(definition.CellX * tileMap.TileWidth, definition.CellY * tileMap.TileHeight, tileMap.TileWidth, tileMap.TileHeight),
                                Color.White
                            );
                        }
                    }
                }
            }
        }

        private void LoadFontFor(TextLabelComponent component)
        {
            var key = $"{component.FontName} {component.FontSize}";
            if (!allFonts.ContainsKey(key))
            {
                var font = this.LoadFont(component.FontName, component.FontSize);
                this.allFonts[key] = font;
            }

            this.entityFonts[component.Parent] = this.allFonts[key];
        }
    }
}