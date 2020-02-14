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
            
            foreach (var puffinAction in PuffinGame.LatestInstance.ActionToKeys.Keys)
            {
                var keyList = PuffinGame.LatestInstance.ActionToKeys[puffinAction];
                foreach (var key in keyList)
                {
                    if (keyboard.IsKeyDown(key) && !keysDown.Contains(key))
                    {
                        EventBus.LatestInstance.Broadcast(EventBusSignal.ActionPressed, puffinAction);
                        keysDown.Add(key);
                    }
                    else if (!keyboard.IsKeyDown(key) && keysDown.Contains(key))
                    {
                        EventBus.LatestInstance.Broadcast(EventBusSignal.ActionReleased, puffinAction);
                        keysDown.Remove(key);
                    }
                }   
            }
        }
    }
}