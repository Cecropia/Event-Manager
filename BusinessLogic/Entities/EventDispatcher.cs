using EventManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // If there is already a synchronous subscription then raise an exception
            if (subscription.Synchronous)
            {
                Subscription syncSubscription = GetSynchronousSubscription(subscription.EventName);
                if (syncSubscription != null)
                {
                    string check =
                        // check appsettings
                        syncSubscription.IsExternal ? "appsettings.json 'EventManager' block" :
                        // check  RegisterLocal methods
                        "your 'EventDispatcher.RegisterLocal' calls";

                    throw new ArgumentException($"EventDispatcher.Register: '{syncSubscription.Subscriber.Name}' already exists for '{syncSubscription.EventName}'.\nOnly one Synchronous Subscription is allowed. \nCheck {check}, for 'Synchronous: true'.");
                }
            }

            if (eventSubscriptions.TryGetValue(subscription.EventName, out var subscriptions))
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
        /// Allows to assign a function to be executed when an Event is called.
        /// </summary>
        /// <param name="eventName">Name of the event to attach the callback.</param>
        /// <param name="callback">Lambda that will be executed when the event is fired,</param>
        /// <param name="synchronous"></param>
        public void RegisterLocal(string eventName, Func<Event, HttpResponseMessage> callback, bool synchronous = false)
        {
            List<Func<Event, HttpResponseMessage>> callbacks = new List<Func<Event, HttpResponseMessage>>
            {
                callback
            };

            Subscriber subscriber = new Subscriber(callback.Method.Name)
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
                IsExternal = false,
                Synchronous = synchronous
            };

            Register(subscription);

        }

        /// <summary>
        /// This is a simplified version of the <see cref="EventDispatcher.Dispatch(Event,List{EventManager.BusinessLogic.Entities.Subscription})"/> method. But instead
        /// of accepting a complete <see cref="Event"/>, it takes the event name and payload and proceeds to build
        /// an event out of these.
        ///
        /// Optionally:
        /// - urlTemplateValues dictionary that will be used to replace the templateKeys in the
        /// endpoint. By default this dictionary is null.
        /// - subscriptions <see cref="List{Subscription}"/> that will be used to replace the local list of destinations used as
        /// endpoint. By default the List is null.
        ///
        /// The event dispatched with this method will have a `Timestamp` of `DateTime.UtcNow`, and the `ExtraParams`
        /// will be `null. If you need to specify any of these then use the normal `Dispatch` method.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventPayload"></param>
        /// <param name="urlTemplateValues">dictionary that will be used to replace the templateKeys in the
        ///     endpoint. By default this dictionary is null.</param>
        /// <param name="subscriptions"><see cref="List{Subscription}"/> that will be used to replace the local list of destinations used as
        ///     endpoint. By default the List is null.</param>
        /// <param name="paramsList">takes a list of key value pairs used for further configure the behavior of the dispatch logic</param>
        public void SimpleDispatch(string eventName, string eventPayload, Dictionary<string, string> urlTemplateValues = null,List<Subscription> subscriptions = null, List<KeyValuePair<string, string>> paramsList = null)
        {
            this.Dispatch(new Event
            {
                Name = eventName,
                Payload = eventPayload,
                Timestamp = DateTime.UtcNow,
                ExtraParams = null,
                UrlTemplateValues = urlTemplateValues
            }, subscriptions, paramsList);
        }

        /// <summary>
        /// Fire an Event to be listened by a Subscription
        ///
        /// Optionally:
        /// - subscriptions <see cref="List{Subscription}"/> that will be used to replace the local list of destinations used as
        /// endpoint. By default the List is null.
        ///
        /// </summary>
        /// <param name="e">Event object</param>
        /// <param name="subscriptions"><see cref="List{Subscription}"/> that will be used to replace the local list of destinations used as
        /// endpoint. By default the List is null.</param>
        /// <param name="paramsList">takes a list of key value pairs used for further configure the behavior of the dispatch logic</param>
        public HttpResponseMessage Dispatch(Event e, List<Subscription> subscriptions = null, List<KeyValuePair<string,string>> paramsList = null)
        {

            Log.Debug($"EventDispatcher.Dispatch: Dispatching event with name '{e.Name}'");

            if(subscriptions == null && eventSubscriptions.TryGetValue(e.Name, out var eventSubscription)){
                subscriptions = eventSubscription;
            }

            subscriptions = subscriptions ?? new List<Subscription>();

            foreach (Subscription subscription in subscriptions.Where(x => !x.Synchronous))
            {
                Log.Debug($"EventDispatcher.Dispatch: Enqueuing item for subscriber '{subscription.Subscriber.Name}', for event '{e.Name}'");
                QueueItem queueItem = new QueueItem()
                {
                    Guid = Guid.NewGuid(),
                    Event = e,
                    Subscription = subscription,
                    Timestamp = DateTime.Now,
                    ParamsList = paramsList
                };

                queue.Add(queueItem);
            }
            // move to last because the return will break the other Queue items
            Subscription synchronousSubscription = subscriptions.FirstOrDefault(s => s.Synchronous);

            // if there is no synchronous event then just return an empty response with 200 status code
            if (synchronousSubscription == null) return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            Log.Debug($"EventDispatcher.Dispatch: Executing Synchronous call for Event: '{e.Name}'");
            return synchronousSubscription.SendEvent(e, paramsList).Result;
        }

        /// <summary>
        /// Get if exists a Synchronous Subscription per EventName
        /// </summary>
        /// <param name="eventName">Name of the Event</param>
        /// <returns>Subscription</returns>
        public Subscription GetSynchronousSubscription(string eventName)
        {
            Subscription syncSubscription = null;
            eventSubscriptions.TryGetValue(eventName, out var subscriptions);
            if (subscriptions != null)
            {
                syncSubscription = subscriptions.FirstOrDefault(s => s.Synchronous == true);
            }
            return syncSubscription;
        }
    }
}
