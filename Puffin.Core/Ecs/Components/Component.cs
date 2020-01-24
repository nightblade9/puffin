using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A base component class. Provides a reference to the parent entity.
    /// </summary>
    public abstract class Component
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