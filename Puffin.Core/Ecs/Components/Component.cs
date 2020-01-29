using System;

namespace Puffin.Core.Ecs.Components
{
    /// <summary>
    /// A base component class. Don't add it to entities directly, it has no functionality
    /// To create your own components, create subclasses of <c>Component</c>.
    /// </summary>
    public abstract class Component
    {
        internal readonly Entity Parent;

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