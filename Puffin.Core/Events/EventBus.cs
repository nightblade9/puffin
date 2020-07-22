using System;
using System.Collections.Generic;

namespace Puffin.Core.Events
{
    /// <summary>
    /// An event bus, which allos you to subscribe to and broadcast events, including internal Puffin events
    /// (such as the arrow keys being pressed).
    /// - Event buses life-cycles are tied to a specific scene; changing scene, creates a new event bus.
    /// - Events must be an enum type, and the callbacks must accept a single object as the data.
    /// - Note that the event bus is disposed and recreated whenever a scene changes.
    /// </summary>
    public class EventBus : IDisposable
    {

        // event name => callbacks. Each callback has an optional parameter (data).
        private readonly IDictionary<Enum, List<Action<object>>> subscribers = new Dictionary<Enum, List<Action<object>>>();

        /// <summary>
        /// Broadcasts a new event to all subscribers of that event.
        /// </summary>
        /// <param name="signal">The enum value of the signal. Any enum works.</param>
        /// <param name="data">Any data to pass to subscribres (can be null).</param>
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

        /// <summary>
        /// Subscribe a callback to a specific signal. The callback must receive an object (optional data).
        /// </summary>
        /// <param name="signal">The enum value of the signal. Any enum works.</param>
        /// <param name="callback">The callback to invoke whenever the signal is broadcast.</param>
        public void Subscribe(Enum signal, Action<object> callback)
        {
            if (!subscribers.ContainsKey(signal))
            {
                subscribers[signal] = new List<Action<object>>();
            }

            subscribers[signal].Add(callback);
        }

        /// <summary>
        /// Unsubscribe a callback to a specific signal.
        /// </summary>
        /// <param name="signal">The enum value of the signal. Any enum works.</param>
        /// <param name="callback">The callback to unsubscribe from the specified signal.</param>
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
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