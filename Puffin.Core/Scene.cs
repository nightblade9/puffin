﻿using System;
using System.Collections.Generic;
using System.Linq;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Systems;
using Puffin.Core.IO;

namespace Puffin.Core
{     
    public class Scene : IKeyboardProvider, IDisposable
    {
        // public for testability; would be protected otherwise
        public Action OnMouseClick;

        private IMouseProvider mouseProvider;
        private IKeyboardProvider keyboardProvider;
        private ISystem[] systems = new ISystem[0];
        private DrawingSystem drawingSystem;
        private List<Entity> entities = new List<Entity>();

        public Tuple<int, int> MouseCoordinates { get { return this.mouseProvider.MouseCoordinates; }}

        public Scene()
        {
            EventBus.LatestInstance.Subscribe(EventBusSignal.MouseClicked, onMouseClick);
        }

        public void Initialize(ISystem[] systems,IMouseProvider mouseProvider, IKeyboardProvider keyboardProvider)
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
        }

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
        /// Internal method that calls `Update` on all systems in this scene.
        /// </summary>
        public void OnUpdate(TimeSpan elapsed)
        {
            foreach (var system in this.systems)
            {
                system.OnUpdate(elapsed);
            }

            this.Update();
        }

        /// <summary>
        /// Internal method that calls `Draw` on the drawing system.
        /// </summary>
        public void OnDraw(TimeSpan elapsed)
        {
            this.drawingSystem.OnDraw(elapsed);
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

        private void onMouseClick(object data)
        {
            if (this.OnMouseClick != null)
            {
                this.OnMouseClick.Invoke();
            }
        }
    }
}
