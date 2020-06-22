using EventManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
        /// Allows to assing a function to be executed when an Event is called.
        /// </summary>
        /// <param name="eventName">Name of the event to attach the callback.</param>
        /// <param name="callback">Lambda that will be executed when the event is fired,</param>
        public void RegisterLocal(string eventName, Action<Event> callback)
        {
            List<Action<Event>> callbacks = new List<Action<Event>>();

            callbacks.Add(callback);

            Subscriber subscriber = new Subscriber()
            {
                Config = new SubscriberConfig
                {
                    MaxTries = 3,
                    RequestRate = 100
                }
            };

            Subscription subscription = new Subscription()
            {
                Subscriber = subscriber,
                EventName = eventName,
                Method = HttpMethod.Post,
                EndPoint = EventManagerConstants.EventReceptionPath,
                CallBacks = callbacks,
                IsExternal = false
            };

            Register(subscription);

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
