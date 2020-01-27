using System;
using System.Collections.Generic;
using System.Linq;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;
using Puffin.Core.Tiles;

namespace Puffin.Core
{
    /// <summary>
    /// A scene or screen in your game. This is where you add entities with functionality
    /// to implement your game's logic.
    /// </summary>
    public class Scene : IDisposable
    {
        public float Fps { get; private set; }
        public uint BackgroundColour = 0x000000; // black

        internal Action OnMouseClick;

        private IMouseProvider mouseProvider;
        private IKeyboardProvider keyboardProvider;
        private ISystem[] systems = new ISystem[0];
        private DrawingSystem drawingSystem;
        private List<Entity> entities = new List<Entity>();
        // Drawn in the order added
        private List<TileMap> tileMaps = new List<TileMap>();

        // A date and a number of draw calls to calculate FPS
        private DateTime lastFpsUpdate = DateTime.Now;
        private int drawsSinceLastFpsCount = 0;

        public Tuple<int, int> MouseCoordinates { get { return this.mouseProvider.MouseCoordinates; }}

        public Scene()
        {
            EventBus.LatestInstance.Subscribe(EventBusSignal.MouseClicked, onMouseClick);
        }

        /// <summary>
        /// Adds an entity to the current scene so that it starts functioning (based on its components).
        /// </summary>
        public void Add(Entity entity)
        {
            this.entities.Add(entity);
            
            // if initialized, notify systems
            if (this.systems.Length > 0)
            {
                foreach (var system in this.systems)
                {
                    system.OnAddEntity(entity);
                }
            }
        }

        /// <summary>
        /// Remove an entity from the scene; it will no longer be rendered, updated, etc.
        /// </summary>
        public void Remove(Entity entity)
        {
            this.entities.Remove(entity);
            foreach (var system in this.systems)
            {
                system.OnRemoveEntity(entity);
            }
        }

        /// <summary>
        /// Adds a tilemap to the scene; it will be drawn. Any tiles with no value set, won't be drawn.
        /// Note that tilemaps are drawn in the order added.
        /// </summary>
        public void Add(TileMap tileMap)
        {
            this.tileMaps.Add(tileMap);
            if (this.drawingSystem != null)
            {
                this.drawingSystem.OnAddTileMap(tileMap);
            }
        }

        /// <summary>
        /// Removes a tilemap from the scene; it will no longer be drawn.
        /// </summary>
        public void Remove(TileMap tileMap)
        {
            this.drawingSystem.OnRemoveTileMap(tileMap);
        }

        /// <summary>
        /// Return true if any of a specific action's keys are pressed.
        /// </summary>        
        public bool IsActionDown(Enum action)
        {
            return this.keyboardProvider.IsActionDown(action);
        }

        /// <summary>
        /// A method that's called every time Update is called by the game engine.
        /// Override it to do things "every frame."
        /// </summary>
        public virtual void Update()
        {
            
        }
        public void Dispose()
        {
            if (EventBus.LatestInstance != null)
            {
                EventBus.LatestInstance.Dispose();
            }

            // Reset EventBus.LatestIntance
            new EventBus();
        }

        /// <summary>
        /// Internal method that calls `Update` on all systems in this scene.
        /// </summary>
        internal void OnUpdate(TimeSpan elapsed)
        {
            foreach (var system in this.systems)
            {
                system.OnUpdate(elapsed);
            }

            var timeDiff = (DateTime.Now - lastFpsUpdate).TotalSeconds;
            if (timeDiff >= 1)
            {
                this.Fps = (float)(drawsSinceLastFpsCount / timeDiff);
                this.drawsSinceLastFpsCount = 0;
                this.lastFpsUpdate = DateTime.Now;
                Console.WriteLine($"{Fps} fps");
            }

            this.Update();
        }

        /// <summary>
        /// Internal method that calls `Draw` on the drawing system.
        /// </summary>
        internal void OnDraw(TimeSpan elapsed)
        {
            drawsSinceLastFpsCount++;
            this.drawingSystem.OnDraw(elapsed, this.BackgroundColour);
        }

        // Separate from the constructor and internal because only we call it; subclasses of
        // Scene don't need to know about this.
        internal void Initialize(ISystem[] systems, IMouseProvider mouseProvider, IKeyboardProvider keyboardProvider)
        {
            this.drawingSystem = systems.Single(s => s is DrawingSystem) as DrawingSystem;
            this.systems = systems;
            
            this.mouseProvider = mouseProvider;
            this.keyboardProvider = keyboardProvider;

            // If called after AddEntity, add entities we know about
            foreach (var entity in this.entities)
            {
                foreach (var system in this.systems)
                {
                    system.OnAddEntity(entity);
                }
            }

            // Initialize tilemaps' sprites
            foreach (var tileMap in this.tileMaps)
            {
                this.drawingSystem.OnAddTileMap(tileMap);
            }
        }

        private void onMouseClick(object data)
        {
            if (this.OnMouseClick != null)
            {
                this.OnMouseClick.Invoke();
            }
        }
    }
}
