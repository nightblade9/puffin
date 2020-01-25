using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using SpriteFontPlus;

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
        private IDictionary<Entity, SpriteFont> entityFonts = new Dictionary<Entity, SpriteFont>();
        
        // "name, size" => font
        private IDictionary<string, SpriteFont> allFonts = new Dictionary<string, SpriteFont>();
        
        // TODO: maybe content pipeline is a good thing, amirite? If so, use LoadContent to load sprites
        private GraphicsDevice graphics;
        private SpriteBatch spriteBatch;

        public MonoGameDrawingSurface(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
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
        }

        public void RemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
            this.entitySprites.Remove(entity);
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