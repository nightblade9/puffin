using Puffin.Core;
using Puffin.Core.Ecs.Systems;
using Puffin.Infrastructure.MonoGame.Drawing;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Puffin.Infrastructure.MonoGame.IO;
using Puffin.Core.IO;
using Ninject;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;

namespace Puffin.Infrastructure.MonoGame
{
    public class PuffinGame : Game
    {
        /// <summary>
        /// A mapping of in-game actions to the (MonoGame) keyboard keys that map to them.
        /// PuffinGame ships with default mappings for all actions; you can override these
        /// to change keyboard bindings, or expose them in a UI and allow users to arbitrarily
        /// override keyboard mappings (for accessibility).
        /// <summary>
        public Dictionary<Enum, List<Keys>> actionToKeys = new Dictionary<Enum, List<Keys>>() {
            { PuffinAction.Up, new List<Keys>() { Keys.W, Keys.Up } },
            { PuffinAction.Down, new List<Keys>() { Keys.S, Keys.Down } },
            { PuffinAction.Left, new List<Keys>() { Keys.A, Keys.Left } },
            { PuffinAction.Right, new List<Keys>() { Keys.D, Keys.Right } },
        };

        public static PuffinGame LatestInstance;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont defaultFont;
        private Scene currentScene;
        private IMouseProvider mouseProvider;
        private IKeyboardProvider keyboardProvider;


        public PuffinGame(int gameWidth, int gameHeight)
        {
            PuffinGame.LatestInstance = this;
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            DependencyInjection.Kernel.Bind<IMouseProvider>().To<MonoGameMouseProvider>().InSingletonScope();
            this.mouseProvider = DependencyInjection.Kernel.Get<IMouseProvider>();

            DependencyInjection.Kernel.Bind<IKeyboardProvider>().To<MonoGameKeyboardProvider>().InSingletonScope();
            this.keyboardProvider = DependencyInjection.Kernel.Get<IKeyboardProvider>();

            this.graphics.PreferredBackBufferWidth = gameWidth;
            this.graphics.PreferredBackBufferHeight = gameHeight;
        }

        public void ShowScene(Scene s)
        {
            if (this.currentScene != null)
            {
                this.currentScene.Dispose();
            }
            
            var drawingSurface = new MonoGameDrawingSurface(this.GraphicsDevice, spriteBatch, this.defaultFont);

            var systems = new ISystem[]
            {
                new FourWayMovementSystem(),
                new DrawingSystem(drawingSurface),
            };

            s.Initialize(systems, this.mouseProvider, this.keyboardProvider);

            this.currentScene = s;
        }

        /// <summary>
        /// Called when your game is ready to run (graphics initialized, etc.)
        /// </summary>
        virtual protected void Ready()
        {

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Not used since we currently load sprites outside the pipeline
            this.defaultFont = Content.Load<SpriteFont>("OpenSans");

            this.Ready();
        }

        protected override void Update(GameTime gameTime)
        {
            this.mouseProvider.Update();
            this.keyboardProvider.Update();
            this.currentScene?.OnUpdate(gameTime.ElapsedGameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: pass in <= 150ms increments if too much time elapsed
            this.currentScene?.OnDraw(gameTime.ElapsedGameTime);
            base.Draw(gameTime);
        }
    }
}