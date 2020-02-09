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
    /// <summary>
    /// Manages scenes.  You can set the size and background colour of your game.
    /// Subclass this to create the entry-point to your game.
    /// </summary>
    public abstract class PuffinGame : Game
    {
        /// <summary>
        /// A mapping of in-game actions to the (MonoGame) keyboard keys that map to them.
        /// PuffinGame ships with default mappings for all actions; you can override these
        /// to change keyboard bindings, or expose them in a UI and allow users to arbitrarily
        /// override keyboard mappings (for accessibility).
        /// </summary>
        public Dictionary<Enum, List<Keys>> ActionToKeys = new Dictionary<Enum, List<Keys>>() {
            { PuffinAction.Up, new List<Keys>() { Keys.W, Keys.Up } },
            { PuffinAction.Down, new List<Keys>() { Keys.S, Keys.Down } },
            { PuffinAction.Left, new List<Keys>() { Keys.A, Keys.Left } },
            { PuffinAction.Right, new List<Keys>() { Keys.D, Keys.Right } },
        };

        internal static PuffinGame LatestInstance { get; private set; }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Scene currentScene;
        private IMouseProvider mouseProvider;
        private IKeyboardProvider keyboardProvider;

        /// <summary>
        /// Creates a new game with the specified window size.
        /// </summary>
        /// <param name="gameWidth">The width of the game window</param>
        /// <param name="gameHeight">The height of the game window</param>
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

        /// <summary>
        /// Switch to a new scene instance. The current scene gets disposed.
        /// </summary>
        public void ShowScene(Scene s)
        {
            if (this.currentScene != null)
            {
                this.currentScene.Dispose();
            }
            
            var drawingSurface = new MonoGameDrawingSurface(this.GraphicsDevice, spriteBatch);

            var systems = new ISystem[]
            {
                new MovementSystem(),
                new OverlapSystem(),
                new MouseOverlapSystem(this.mouseProvider),
                new MouseSystem(),
                new AudioSystem(new MonoGameAudioPlayer()),
                new DrawingSystem(drawingSurface),
            };

            s.Initialize(systems, this.mouseProvider, this.keyboardProvider);

            this.currentScene = s;
        }

        /// <summary>
        /// Called when your game is ready to run (graphics initialized, etc.)
        /// Implement this to add entities that load sprites, etc. for your game.
        /// </summary>
        virtual protected void Ready()
        {

        }

        /// <summary>Overridden from MonoGame, please ignore.</summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>Overridden from MonoGame, please ignore.</summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Not used since we currently load sprites outside the pipeline
            this.Ready();
        }

        /// <summary>Overridden from MonoGame, please ignore.</summary>
        protected override void Update(GameTime gameTime)
        {
            this.mouseProvider.Update();
            this.keyboardProvider.Update();
            this.currentScene?.OnUpdate(gameTime.ElapsedGameTime);
            base.Update(gameTime);
        }

        /// <summary>Overridden from MonoGame, please ignore.</summary>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: pass in <= 150ms increments if too much time elapsed
            this.currentScene?.OnDraw(gameTime.ElapsedGameTime);
            base.Draw(gameTime);
        }
    }
}