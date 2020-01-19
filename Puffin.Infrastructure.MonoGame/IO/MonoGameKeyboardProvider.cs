using Microsoft.Xna.Framework.Input;
using Puffin.Core.IO;

namespace Puffin.Infrastructure.MonoGame.IO
{
    /// <summary>
    /// An implementation of IKeyboardProvider using MonoGame's Keyboard class.
    public class MonoGameKeyboardProvider : IKeyboardProvider
    {
        public bool IsActionDown(PuffinAction action)
        {
            var keyboard = Keyboard.GetState();
            var keysForAction = PuffinGame.LatestInstance.actionToKeys[action];

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
            // Not needed for keyboard just yet
        }
    }
}