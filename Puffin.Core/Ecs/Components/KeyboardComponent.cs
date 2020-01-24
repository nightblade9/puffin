using Puffin.Core.IO;
using Ninject;
using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A keyboard component; provides information about which actions are being pressed.
    /// Actions are a simple enum that map to one or more keyboard keys. 
    /// For examples, see the Puffin documentation or unit tests.
    /// </summary>
    public class KeyboardComponent : Component
    {
        private IKeyboardProvider provider;

        public KeyboardComponent(Entity parent) : base(parent)
        {
            this.provider = DependencyInjection.Kernel.Get<IKeyboardProvider>();
        }

        /// <summary>
        /// Returns true if any of the keys mapped to this action are currently pressed down.
        /// Returns false otherwise.
        /// </summary>
        public bool IsActionDown(Enum action)
        {
            return this.provider.IsActionDown(action);
        }
    }
}