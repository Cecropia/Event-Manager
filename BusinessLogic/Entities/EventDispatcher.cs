using Serilog;
using System;
using System.Collections.Generic;

namespace EventManager.BusinessLogic.Entities
{
    public sealed class EventDispatcher
    {
        private static readonly Lazy<EventDispatcher> lazy = new Lazy<EventDispatcher>(() => new EventDispatcher());
        public static EventDispatcher Instance { get { return lazy.Value; } }

        private static Queue queue;

        // Dictionary is Thread safe.
        private static Dictionary<string, List<Subscription>> eventSubscriptions;


        /// <summary>
        /// EventDispatcher Constructor, only invoked through Instance
        /// </summary>
        private EventDispatcher()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();

            queue = Queue.Instance;
            eventSubscriptions = new Dictionary<string, List<Subscription>>();
        }

        /// <summary>
        /// Add a Subscription to wait for an Event
        /// </summary>
        /// <param name="subscription">Subscription object</param>
        public void Register(Subscription subscription)
        {
            Log.Debug("EventDispatcher.Register");
            List<Subscription> subscriptions;
            if (eventSubscriptions.TryGetValue(subscription.EventName, out subscriptions))
            {
                subscriptions.Add(subscription);
            }
            else
            {
                subscriptions = new List<Subscription>
                {
                    subscription
                };
                eventSubscriptions.Add(subscription.EventName, subscriptions);
            }
        }

        /// <summary>
        /// Fire an Event to be listened by a Subscription
        /// </summary>
        /// <param name="e">Event object</param>
        public void Dispatch(Event e)
        {
            Log.Debug("EventDispatcher.Dispatch");
            List<Subscription> subscriptions;
            if (eventSubscriptions.TryGetValue(e.Name, out subscriptions))
            {
                Log.Debug("EventDispatcher.Dispatch: Found value");
                foreach (Subscription subscription in subscriptions)
                {
                    QueueItem queueItem = new QueueItem()
                    {
                        Guid = Guid.NewGuid(),
                        Event = e,
                        Subscription = subscription,
                        Timestamp = DateTime.Now
                    };

                    queue.Add(queueItem);
                }
            }
            else
            {
                Log.Debug("EventDispatcher.Dispatch: Value not found");
            }
        }
    }
}
