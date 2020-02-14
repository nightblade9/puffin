using System;
using System.Collections.Generic;

namespace Puffin.Core.Events
{
    /// <summary>
    /// An event bus; used internally by Puffin. You can use it to subscribe to and broadcast events.
    /// Events must be an enum type, and the callbacks must accept a single object as the data.
    /// Note that the event bus is disposed and recreated whenever a scene changes.
    /// </summary>
    public class EventBus : IDisposable
    {
        public static EventBus LatestInstance { get; private set; } = new EventBus();

        // event name => callbacks. Each callback has an optional parameter (data).
        private IDictionary<Enum, List<Action<object>>> subscribers = new Dictionary<Enum, List<Action<object>>>();

        public EventBus()
        {
            LatestInstance = this;
        }

        public void Broadcast(Enum signal, object data = null)
        {
            if (subscribers.ContainsKey(signal))
            {
                // TODO: can be done in parallel if needed. Copy so if someone has
                // a mouse-click handler that changes scenes, it doesn't throw a
                // modified-during-enumeration exception.
                foreach (var subscriber in subscribers[signal].ToArray())
                {
                    subscriber.Invoke(data);
                }
            }
        }

        public void Subscribe(Enum signal, Action<object> callback)
        {
            if (!subscribers.ContainsKey(signal))
            {
                subscribers[signal] = new List<Action<object>>();
            }

            subscribers[signal].Add(callback);
        }

        public void Unsubscribe(Enum signal, Action<object> callback)
        {
            if (subscribers.ContainsKey(signal))
            {
                subscribers[signal].Remove(callback);
            }
            else
            {
                throw new ArgumentException($"There are no subscribers for the {signal} signal.");
            }
        }

        public void Dispose()
        {
            // Perhaps just being over-cautious here
            foreach (var signal in subscribers.Keys)
            {
                this.subscribers[signal].Clear();
            }

            this.subscribers.Clear();
        }
    }
}