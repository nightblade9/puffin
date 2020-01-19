using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using Ninject;

namespace Puffin.Core.Ecs
{
    public class KeyboardComponent : Component
    {
        private IKeyboardProvider provider;

        public KeyboardComponent(Entity parent) : base(parent)
        {
            this.provider = DependencyInjection.Kernel.Get<IKeyboardProvider>();
        }

        public bool IsActionDown(PuffinAction action)
        {
            return this.provider.IsActionDown(action);
        }
    }
}