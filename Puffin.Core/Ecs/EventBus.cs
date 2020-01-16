using System;
using System.Collections.Generic;

namespace Puffin.Core.Ecs
{
    class EventBus : IDisposable
    {
        public static EventBus LatestInstance { get; private set; } = new EventBus();

        // event name => callbacks. Each callback has an optional parameter (data).
        private IDictionary<EventBusSignal, List<Action<object>>> subscribers = new Dictionary<EventBusSignal, List<Action<object>>>();

        public EventBus()
        {
            LatestInstance = this;
        }

        public void Broadcast(EventBusSignal signal, object data = null)
        {
            if (subscribers.ContainsKey(signal))
            {
                // TODO: can be done in parallel if needed
                foreach (var subscriber in subscribers[signal])
                {
                    subscriber.Invoke(data);
                }
            }
        }

        public void Subscribe(EventBusSignal signal, Action<object> callback)
        {
            if (!subscribers.ContainsKey(signal))
            {
                subscribers[signal] = new List<Action<object>>();
            }

            subscribers[signal].Add(callback);
        }

        public void Unsubscribe(EventBusSignal signal, Action<object> callback)
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