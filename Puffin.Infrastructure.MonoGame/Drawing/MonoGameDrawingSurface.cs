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
using System.Text;

namespace Puffin.Infrastructure.MonoGame.Drawing
{
    /// <summary>
    /// A drawing surface for MonoGame (a wrapper around SpriteBatch).
    /// </summary>
    internal class MonoGameDrawingSurface : IDrawingSurface, IDisposable
    {
        public static MonoGameDrawingSurface LatestInstance { get; private set; }

        private const int DefaultFontSize = 24;

        private readonly EventBus eventBus;
        private SpriteFont defaultFont;

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

        // Draw to this surface, then stretch/shrink to draw to screen
        private readonly RenderTarget2D sceneRenderTarget;
        // Sub-scenes are drawn here (on a transparent render target).
        private readonly RenderTarget2D subSceneRenderTarget;

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
            this.LoadDefaultFont();

            this.sceneRenderTarget = new RenderTarget2D(this.graphics, PuffinGame.LatestInstance.GameWidth, PuffinGame.LatestInstance.GameHeight);
            this.subSceneRenderTarget = new RenderTarget2D(this.graphics, PuffinGame.LatestInstance.GameWidth, PuffinGame.LatestInstance.GameHeight);

            this.eventBus.Subscribe(EventBusSignal.LabelFontChanged, (data) =>
            {
                var label = data as TextLabelComponent;
                this.LoadFontFor(label);
                this.UpdatePixelWidth(label);
            });

            this.eventBus.Subscribe(EventBusSignal.LabelTextChanged, (data) =>
            {
                this.UpdatePixelWidth(data as TextLabelComponent);
            });

            this.eventBus.Subscribe(EventBusSignal.SpriteChanged, (data) =>
            {
                var sprite = data as SpriteComponent;
                this.AddMonoGameSpriteFor(sprite.Parent);
            });

            this.eventBus.Subscribe(EventBusSignal.TilesetSpriteChanged, (data) =>
            {
                var tilemap = data as TileMap;
                this.AddTileMap(tilemap);
            });

            this.eventBus.Subscribe(EventBusSignal.BackgroundSet, (data) => 
            {
                this.backgroundSprite = LoadImage(data.ToString());
            });

            // Make sure renderTarget is always transparent. Otherwise, the contents will actually be solid black.
            // That means when we draw the (empty) subscene render target, we get garbage on screen.
            this.graphics.SetRenderTarget(subSceneRenderTarget);
            this.graphics.Clear(Color.Transparent);
            this.graphics.SetRenderTarget(null);
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
                    this.UpdatePixelWidth(entity.Get<TextLabelComponent>());
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

        /// <summary>
        /// Draws all the entities, tilemaps, etc. to the appropriate render target - there's one for
        /// scenes, and a different one for subscenes. Then later, a call to FlushToScreen draws them
        /// in order, with transparent backgrounds on the subscene.
        /// </summary>
        public void DrawAll(int backgroundColour, string backgroundImage = "", bool clearDisplay = true)
        {
            // Drawing a main/parent scene
            if (clearDisplay)
            {
                this.graphics.SetRenderTarget(sceneRenderTarget);

                this.graphics.Clear(BgrToRgba(backgroundColour));
                
                if (this.backgroundSprite != null)
                {
                    this.spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    this.spriteBatch.Draw(this.backgroundSprite, Vector2.Zero, Color.White);
                    this.spriteBatch.End();
                }
            }
            // Note: there's no "else" clause clearing the sub-scene buffer, because doing that
            // here clears the content! If it turns out that this is important, we may need to
            // add another call (all the way from PuffinGame down to here, like FlushToScreen)
            // that explicitly clears both/all render targets.
            
            // As of writing, this appears to work because we always draw a full/filled parent
            // scene first (background colour or sprite) so you won't notice the sub-scene buffer
            // "bleeding" anything from not being cleared.

            var lastActiveCamera = this.cameras.LastOrDefault();
            MonoGameCamera camera = null;
            if (lastActiveCamera != null)
            {
                camera = this.entityCameras[lastActiveCamera];
                var cameraComponent = lastActiveCamera.Get<CameraComponent>();
                // This smells. How do we synch properties?
                camera.Zoom = new Vector2(cameraComponent.Zoom, cameraComponent.Zoom);
            }

            this.spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera?.TransformationMatrix, blendState: BlendState.NonPremultiplied);
            this.DrawTileMaps();
            this.DrawEntities(this.entities);
            this.spriteBatch.End();

            this.spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
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

        /// <summary>
        /// We've finished drawng parent/child scenes to their appropriate render targets; now draw them
        /// on-screen, in order. Note that this is necessary because <c>graphics.SetRenderTarget(null)</c>
        /// clears what was previously drawn and fills the screen with black.
        public void FlushToScreen()
        {
            // Finished rendering to renderTarget, now scale to draw onto the screen
            this.graphics.SetRenderTarget(null);
            var screenRectangle = new Rectangle(0, 0, PuffinGame.LatestInstance.Width, PuffinGame.LatestInstance.Height);
            var gameRectangle = new Rectangle(0, 0, PuffinGame.LatestInstance.GameWidth, PuffinGame.LatestInstance.GameHeight);
            
            spriteBatch.Begin();
            spriteBatch.Draw(sceneRenderTarget, screenRectangle, gameRectangle, Color.White);
            spriteBatch.Draw(subSceneRenderTarget, screenRectangle, gameRectangle, Color.White);
            spriteBatch.End();
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

        // Hackity-hack: don't talk back ...
        public void LoadDefaultFont()
        {
            var previousDefault = this.defaultFont;

            this.defaultFont = this.LoadFont(PuffinGame.LatestInstance.DefaultFont, DefaultFontSize);
            
            // For every entity using the previous default: use the new default.
            
            // TODO: herp derp previousDefault users only right? No way to tell, though ...
            // BUG: this probably sets ALL things to the default font.
            // For my game, I only use one font, so it Just Works.
            this.allFonts.Clear(); // clear cached default (empty name)
            foreach (var kvp in this.entityFonts)
            {
                this.LoadFontFor(kvp.Key.Get<TextLabelComponent>());
            }
        }

        private void UpdatePixelWidth(TextLabelComponent label)
        {
            var font = this.entityFonts[label.Parent];
            label.WidthInPixels = font.MeasureString(label.Text).X;
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
                File.ReadAllBytes(Path.Combine("Content", "Fonts", $"{fileName}.ttf")), fontSize, 4096, 4096, 
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
                if (entity.DrawColourBeforeSprite)
                {
                    this.DrawColour(entity);
                    this.DrawSprite(entity);
                }
                else
                {
                    this.DrawSprite(entity);
                    this.DrawColour(entity);
                }

                var text = entity.Get<TextLabelComponent>();
                if (text != null)
                {
                    if (!this.entityFonts.ContainsKey(entity))
                    {
                        this.entityFonts[entity] = this.defaultFont;
                    }

                    var font = this.entityFonts[entity];
                    var wrappedText = text.WordWrapWidth > 0 ? WrapText(this.entityFonts[entity], text.Text, text.WordWrapWidth) : text.Text;
                    if (text.OutlineThickness > 0)
                    {
                        this.spriteBatch.DrawString(font, wrappedText, new Vector2(entity.X + text.OffsetX - text.OutlineThickness, entity.Y + text.OffsetY - text.OutlineThickness), BgrToRgba(text.OutlineColour) * text.Alpha);
                        this.spriteBatch.DrawString(font, wrappedText, new Vector2(entity.X + text.OffsetX + text.OutlineThickness, entity.Y + text.OffsetY - text.OutlineThickness), BgrToRgba(text.OutlineColour) * text.Alpha);
                        this.spriteBatch.DrawString(font, wrappedText, new Vector2(entity.X + text.OffsetX - text.OutlineThickness, entity.Y + text.OffsetY + text.OutlineThickness), BgrToRgba(text.OutlineColour) * text.Alpha);
                        this.spriteBatch.DrawString(font, wrappedText, new Vector2(entity.X + text.OffsetX + text.OutlineThickness, entity.Y + text.OffsetY + text.OutlineThickness), BgrToRgba(text.OutlineColour) * text.Alpha);
                    }
                    this.spriteBatch.DrawString(font, wrappedText, new Vector2(entity.X + text.OffsetX, entity.Y + text.OffsetY), BgrToRgba(text.Colour) * text.Alpha);
                }
            }
        }

        // Source: https://stackoverflow.com/a/39349224/8641842
        private static string WrapText(SpriteFont font, string text, int maxLineWidth)
        {
            string[] words = text.Split(' ');
            var sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (word.Contains("\r"))
                {
                    lineWidth = 0f;
                    sb.Append("\r \r" );
                }

                if (lineWidth + size.X < maxLineWidth )
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }

                else
                {
                    if (size.X > maxLineWidth )
                    {
                        if (sb.ToString() == " ")
                        {
                            sb.Append(WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                        else
                        {
                            sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }

            return sb.ToString();
        }

        private void DrawSprite(Entity entity)
        {
            MonoGameSprite monoGameSprite = null;
            this.entitySprites.TryGetValue(entity, out monoGameSprite);
            var sprite = entity.Get<SpriteComponent>();

            if (monoGameSprite != null && sprite.IsVisible)
            {
                this.spriteBatch.Draw(monoGameSprite.Texture, new Vector2(entity.X + sprite.OffsetX, entity.Y + sprite.OffsetY), monoGameSprite.Region, Color.White * sprite.Alpha);
            }
        }

        private void DrawColour(Entity entity)
        {
            var colour = entity.Get<ColourComponent>();
            if (colour != null)
            {
                this.spriteBatch.Draw(whiteRectangle, 
                    new Rectangle((int)entity.X + colour.OffsetX, (int)entity.Y + colour.OffsetY, colour.Width, colour.Height),
                    BgrToRgba(colour.Colour, colour.Alpha));
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
                var fontName = string.IsNullOrWhiteSpace(component.FontName) ? PuffinGame.LatestInstance.DefaultFont : component.FontName;
                var font = this.LoadFont(fontName, component.FontSize);
                this.allFonts[key] = font;
            }

            this.entityFonts[component.Parent] = this.allFonts[key];
        }
    }
}