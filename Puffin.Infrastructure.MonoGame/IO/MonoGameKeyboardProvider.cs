using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using Puffin.Core.IO;

namespace Puffin.Infrastructure.MonoGame.IO
{
    /// <summary>
    /// An implementation of IKeyboardProvider using MonoGame's Keyboard class.
    /// </summary>
    internal class MonoGameKeyboardProvider : IKeyboardProvider
    {
        private List<Enum> keysDown = new List<Enum>();
        private EventBus eventBus;

        public MonoGameKeyboardProvider(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        // Used for simple checks in the scene for key-presses.
        public bool IsActionDown(Enum action)
        {
            var keyboard = Keyboard.GetState();
            var keysForAction = PuffinGame.LatestInstance.ActionToKeys[action];

            foreach (var key in keysForAction)
            {
                if (keyboard.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void Update()
        {
            // Poll for which keys are just pressed/released, and notify appropriatley
            var keyboard = Keyboard.GetState();
            
            foreach (var gameAction in PuffinGame.LatestInstance.ActionToKeys.Keys)
            {
                var keyList = PuffinGame.LatestInstance.ActionToKeys[gameAction];
                foreach (var key in keyList)
                {
                    if (keyboard.IsKeyDown(key) && !keysDown.Contains(key))
                    {
                        this.eventBus.Broadcast(EventBusSignal.ActionPressed, gameAction);
                        keysDown.Add(key);
                    }
                    else if (!keyboard.IsKeyDown(key) && keysDown.Contains(key))
                    {
                        this.eventBus.Broadcast(EventBusSignal.ActionReleased, gameAction);
                        keysDown.Remove(key);
                    }
                }   
            }
        }
    }
}