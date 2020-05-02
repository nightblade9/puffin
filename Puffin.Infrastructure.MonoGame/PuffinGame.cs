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
using Puffin.Core.Events;

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

        /// <summary>
        /// Sets the game to run in full-screen mode (or not).
        /// This can be toggled at any time in-game.
        /// </summary>
        public bool IsFullScreen
        {
            get
            { 
                return this.graphics.IsFullScreen;
            }
            set
            {
                this.graphics.IsFullScreen = value;
                this.graphics.ApplyChanges();
            }
        }
        
        public string DefaultFont = "OpenSans";

        internal static PuffinGame LatestInstance { get; private set; }

        internal bool ShowCollisionAreas { get { return this.showCollisionAreas; } }

        /// <summary>
        /// Set this to true to render collision areas as red transparent rectangles.
        /// </summary>
        protected bool showCollisionAreas = false;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Scene currentScene;

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

            this.graphics.PreferredBackBufferWidth = gameWidth;
            this.graphics.PreferredBackBufferHeight = gameHeight;
        }

        /// <summary>
        /// Switch to a new scene instance. The current scene gets disposed.
        /// </summary>
        public void ShowScene(Scene s)
        {
            this.currentScene?.Dispose();
            this.InitializeSceneSystems(s);
            this.currentScene = s;
        }

        /// <summary>
        /// The display width of the main game window.
        /// </summary>
        public int Width { get { return this.graphics.PreferredBackBufferWidth; } }

        /// <summary>
        /// The display height of the main game window.
        /// </summary>
        public int Height { get { return this.graphics.PreferredBackBufferHeight; } }

        /// <summary>
        /// Called when your game is ready to run (graphics initialized, etc.)
        /// Implement this to add entities that load sprites, etc. for your game.
        /// Make sure you call <c>base.Ready</c> if you override it.
        /// </summary>
        virtual protected void Ready()
        {
            this.currentScene?.Ready();
            this.currentScene?.SubScene?.Ready();
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
            // Parent scene doesn't receive updates while subscene is there.
            if (this.currentScene?.SubScene != null)
            {
                this.currentScene?.SubScene?.OnUpdate(gameTime.ElapsedGameTime);
            }
            else
            {
                this.currentScene?.OnUpdate(gameTime.ElapsedGameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>Overridden from MonoGame, please ignore.</summary>
        protected override void Draw(GameTime gameTime)
        {
            // Always draw parent because we clear drawing surface, wanna see parent
            // if there's a subscene.
            this.currentScene?.OnDraw(gameTime.ElapsedGameTime, true);
            
            // Parent scene doesn't receive draw calls while subscene is there.
            if (this.currentScene?.SubScene != null && this.currentScene.SubScene.CalledReady)
            {
                this.currentScene?.SubScene?.OnDraw(gameTime.ElapsedGameTime, false);
            }

            base.Draw(gameTime);
        }

        private void InitializeSceneSystems(Scene s)
        {
            // Weird, can't go in constructor - no eventbus
            s.EventBus.Subscribe(EventBusSignal.SubSceneShown, (data) =>
            {
                var subScene = data as Scene;
                this.InitializeSceneSystems(subScene);
            });

            // BUG: hiding a sub-scene didn't restore MonoGameDrawingSurface and friends,
            // meaning we're using the old surface; which has an old list of
            // entities/cameras, so the camera stuff gets messed up if the zoom
            // is different (eg. main game zoom=3, options subscene zoom=1).
            s.EventBus.Subscribe(EventBusSignal.SubSceneHidden, (data) =>
            {
                if (data != null)
                {
                    var subScene = data as Scene;
                    subScene.Dispose();
                    // Reset all systems/etc. to point to the parent scene again
                    var parentScene = subScene.ParentScene;
                    parentScene.EventBus.Dispose();
                    parentScene.EventBus = new EventBus();
                    this.InitializeSceneSystems(parentScene);
                }
            });

            var mouseProvider = new MonoGameMouseProvider();
            var keyboardProvider = new MonoGameKeyboardProvider(s.EventBus);
            
            var drawingSurface = new MonoGameDrawingSurface(s.EventBus, this.GraphicsDevice, spriteBatch);

            var systems = new ISystem[]
            {
                new MovementSystem(s),
                new OverlapSystem(),
                new MouseOverlapSystem(mouseProvider),
                new MouseSystem(s.EventBus, mouseProvider),
                new KeyboardSystem(s.EventBus, keyboardProvider),
                new AudioSystem(new MonoGameAudioPlayer(s.EventBus)),
                new DrawingSystem(drawingSurface),
            };

            s.Initialize(systems, mouseProvider, keyboardProvider);

            if (!s.CalledReady)
            {
                s.Ready();
            }
        }
    }
}