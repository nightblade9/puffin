using System;

namespace Puffin.Core.Ecs.Components
{
    public class Component
    {
        protected readonly Entity Parent;

        public Component(Entity parent)
        {
            if (parent == null)
            {
                throw new NullReferenceException(nameof(parent));
            }
            this.Parent = parent;
        }
    }
}