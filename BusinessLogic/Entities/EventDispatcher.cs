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

        public static CustomAuthProviderRegistry customAuthProviderRegistry;

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
            customAuthProviderRegistry = new CustomAuthProviderRegistry();
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
        /// This is a simplified version of the <see cref="EventDispatcher.Dispatch(Event)"/> method. But instead
        /// of accepting a complete <see cref="Event"/>, it takes the event name and payload and proceeds to build
        /// an event out of these.
        /// 
        /// Optionally, also a dictionary of urlTemplateValues that will be used to replace the templateKeys in the
        /// endpoint. By default this dictionary is null.
        /// 
        /// The event dispatched with this method will have a `Timestamp` of `DateTime.UtcNow`, and the `ExtraParams`
        /// will be `null. If you need to specify any of these then use the normal `Dispatch` method.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventPayload"></param>
        public void SimpleDispatch(string eventName, string eventPayload, Dictionary<string, string> urlTemplateValues = null)
        {
            this.Dispatch(new Event
            {
                Name = eventName,
                Payload = eventPayload,
                Timestamp = DateTime.UtcNow,
                ExtraParams = null,
                UrlTemplateValues = urlTemplateValues
            });
        }

        /// <summary>
        /// Fire an Event to be listened by a Subscription
        /// </summary>
        /// <param name="e">Event object</param>
        public void Dispatch(Event e)
        {
            Log.Debug($"EventDispatcher.Dispatch: Dispatching event with name '{e.Name}'");
            List<Subscription> subscriptions;
            if (eventSubscriptions.TryGetValue(e.Name, out subscriptions))
            {
                Log.Debug($"EventDispatcher.Dispatch: Found subscriptions to event '{e.Name}', enqueuing items");
                foreach (Subscription subscription in subscriptions)
                {
                    Log.Debug($"EventDispatcher.Dispatch: Enqueuing item for subscriber '{subscription.Subscriber.Name}', for event '{e.Name}'");
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
                Log.Debug($"EventDispatcher.Dispatch: Nothing will be dispatched - No subscriptions found for event '{e.Name}'");
            }
        }
    }
}
