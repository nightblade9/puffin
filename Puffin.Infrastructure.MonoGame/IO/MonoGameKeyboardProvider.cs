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
            // Bug: if you press and hold a key K on scene S1, and that transitions to
            // scene S2 which also has an OnActionDown handler for K, you end up immediately
            // invoking the key handler on S2 ... when, in fact, the user didn't do anything.
            
            // To fix this bug, when we initialize a new keyboard provider, note the state of
            // the keyboard, and assume that's how it started?
            this.Update(false);
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
            this.Update(true);
        }

        public void Update(bool triggerEvents)
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
                        keysDown.Add(key);
                        if (triggerEvents)
                        {
                            this.eventBus.Broadcast(EventBusSignal.ActionPressed, gameAction);
                        }
                    }
                    else if (!keyboard.IsKeyDown(key) && keysDown.Contains(key))
                    {
                        keysDown.Remove(key);
                        if (triggerEvents)
                        {
                            this.eventBus.Broadcast(EventBusSignal.ActionReleased, gameAction);
                        }
                    }
                }   
            }
        }

        public void Reset()
        {
            this.keysDown.Clear();
        }
    }
}