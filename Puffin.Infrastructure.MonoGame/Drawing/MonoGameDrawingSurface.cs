using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using SpriteFontPlus;
using System;
using Puffin.Core.Tiles;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    /// <summary>
    /// A drawing surface for MonoGame (a wrapper around SpriteBatch).
    /// </summary>
    internal class MonoGameDrawingSurface : IDrawingSurface, IDisposable
    {
        private readonly SpriteFont defaultFont;

        private IList<Entity> entities = new List<Entity>();

        private IDictionary<Entity, MonoGameSprite> entitySprites = new Dictionary<Entity, MonoGameSprite>();
        private IDictionary<TileMap, Texture2D> tileMapSprites = new Dictionary<TileMap, Texture2D>();

        private IDictionary<Entity, SpriteFont> entityFonts = new Dictionary<Entity, SpriteFont>();        
        // "name, size" => font. Cache of all fonts ever seen so far.
        private IDictionary<string, SpriteFont> allFonts = new Dictionary<string, SpriteFont>();
        
        private readonly GraphicsDevice graphics;
        private readonly SpriteBatch spriteBatch;

        // 1x1 white rectangle, used to draw colour components
        private readonly Texture2D whiteRectangle;

        public MonoGameDrawingSurface(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            whiteRectangle = new Texture2D(graphics, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.defaultFont = this.LoadFont("OpenSans", 24);

            EventBus.LatestInstance.Subscribe(EventBusSignal.LabelFontChanged, (data) =>
            {
                var component = data as TextLabelComponent;
                var key = $"{component.FontName} {component.FontSize}";
                if (!allFonts.ContainsKey(key))
                {
                    var font = this.LoadFont(component.FontName, component.FontSize);
                    this.allFonts[key] = font;
                }

                this.entityFonts[component.Parent] = this.allFonts[key];
            });
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
                // TODO: load the appropriate font or specify the default font
            }
            else if (entity.GetIfHas<ColourComponent>() != null)
            {
                this.entities.Add(entity);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            var monoGameSprite = this.entitySprites[entity];
            monoGameSprite.Dispose();
            this.entitySprites.Remove(entity);
        }

        public void AddTileMap(TileMap tileMap)
        {
            this.tileMapSprites[tileMap] = LoadImage(tileMap.TileImageFile);
        }

        public void RemoveTileMap(TileMap tileMap)
        {
            this.tileMapSprites[tileMap].Dispose();
            this.tileMapSprites.Remove(tileMap);
        }

        public void DrawAll(uint backgroundColour)
        {
            this.graphics.Clear(new Color(backgroundColour + 0xFF000000));
            this.spriteBatch.Begin();

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
                                new Vector2(x * tileMap.TileWidth, y * tileMap.TileHeight),
                                new Rectangle(definition.CellX * tileMap.TileWidth, definition.CellY * tileMap.TileHeight, tileMap.TileWidth, tileMap.TileHeight),
                                Color.White
                            );
                        }
                    }
                }
            }

            foreach (var entity in this.entities)
            {
                var colour = entity.GetIfHas<ColourComponent>();
                if (colour != null)
                {
                    this.spriteBatch.Draw(whiteRectangle, 
                        new Rectangle((int)entity.X, (int)entity.Y, colour.Width, colour.Height),
                        new Color(colour.Colour + 0xFF000000));
                }

                foreach (var monoGameSprite in entitySprites.Values)
                {
                    this.spriteBatch.Draw(monoGameSprite.Texture, new Vector2(entity.X, entity.Y), monoGameSprite.Region, Color.White);
                }

                var text = entity.GetIfHas<TextLabelComponent>();
                if (text != null)
                {
                    if (!this.entityFonts.ContainsKey(entity))
                    {
                        this.entityFonts[entity] = this.defaultFont;
                    }

                    var font = this.entityFonts[entity];
                    this.spriteBatch.DrawString(font, text.Text, new Vector2(entity.X, entity.Y), Color.White);
                }
            }
            
            this.spriteBatch.End();
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
                File.ReadAllBytes(Path.Combine("Content", $"{fileName}.ttf")), fontSize, 1024, 1024, 
                new[] {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic });

            var font = fontBakeResult.CreateSpriteFont(this.graphics);
            return font;
        }
    }
}