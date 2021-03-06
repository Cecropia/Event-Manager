using EventManager.BusinessLogic.Entities;
using EventManager.Data;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;


namespace ExecutableTest
{
    class Program
    {
        static Queue queue;
        static EventDispatcher EventDispatcher;

        static void Main()
        {
            //queue_test();
            event_dispatcher_test();
        }

        public static void queue_test()
        {
            Console.WriteLine("---- Queue Test");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();


            string responseBody = @"
                {
                  'Name'  : 'careers_salesforce_after_skill_created',
                  'Payload':[
                    {'item1':'value1'},
                    {'item2':'value2'}
                  ],
                  'Timestamp': '2020-05-22T21:28:06.496Z',
                  'ExtraParams': { 'name': 'value'},
                }
            ";


            JObject json = JObject.Parse(responseBody);

            var contextValues = new Dictionary<string, IEnumerable<string>> { {"context1", new List<string>() { "1", "2", "3"} }, {"context2", new List<string>() { "1", "2", "3"} } };

            Event ev = new Event()
            {
                Name = (string)json["Name"],
                Timestamp = (DateTime)json["Timestamp"],
                Payload = json["Payload"].ToString(),
                ExtraParams = json["ExtraParams"].ToObject<JObject>(),
                ContextHeaders = contextValues
            };

            Subscriber subscriber = new Subscriber("Subscriber Name 1")
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
                EventName = "event_name",
                Method = HttpMethod.Post,
                EndPoint = EventManagerConstants.EventReceptionPath,
                CallBacks = new List<Func<Event, HttpResponseMessage>>()
            };

            QueueItem q1 = new QueueItem()
            {
                Guid = Guid.NewGuid(),
                Event = ev,
                Subscription = subscription,
                Timestamp = DateTime.Now
            };
            QueueItem q2 = new QueueItem()
            {
                Guid = Guid.NewGuid(),
                Event = ev,
                Subscription = subscription,
                Timestamp = DateTime.Now
            };

            queue = Queue.Instance;

            queue.Add(q2);
            queue.Add(q1);


            Thread.Sleep(4000);

            QueueItem q3 = new QueueItem()
            {
                Guid = Guid.NewGuid(),
                Event = ev,
                Subscription = subscription,
                Timestamp = DateTime.Now
            };

            queue.Add(q3);

            for (int k = 0; k < 1000; k++)
            {
                QueueItem qNew = new QueueItem()
                {
                    Guid = Guid.NewGuid(),
                    Event = ev,
                    Subscription = subscription,
                    Timestamp = DateTime.Now
                };
                queue.Add(qNew);
                //Thread.Sleep(10 + (1000 - k));
                //Thread.Sleep(10);
            }

            Console.ReadKey();
        }


        public static void event_dispatcher_test()
        {
            Console.WriteLine("---- EventDispatcher Test");

            EventDispatcher = EventDispatcher.Instance;

            // overwriting for console output
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            List<Func<Event, HttpResponseMessage>> callbacks = new List<Func<Event, HttpResponseMessage>>();

            Func<Event, HttpResponseMessage> callback = (Event e) => {
                Log.Debug("---- Call from Subscription callback");
                return null;
            };

            callbacks.Add(callback);


            Subscriber subscriber = new Subscriber("Subscriber Name 2")
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
                EventName = "careers_salesforce_after_skill_created",
                Method = HttpMethod.Post,
                EndPoint = EventManagerConstants.EventReceptionPath,
                CallBacks = callbacks,
                IsExternal = false,
                Synchronous = false
            };


            EventDispatcher.Register(subscription);

            EventDispatcher.RegisterLocal("careers_salesforce_after_skill_created", OnEvent1, false);

            EventDispatcher.RegisterLocal("careers_salesforce_after_skill_created", (Event e) =>
            {
                Log.Debug("---- Call from RegisterLocal callback 2");
                return null;
            }, true);



            //-----------------------------------------------//

            Thread.Sleep(4000);


            string responseBody = @"
                {
                  'Name'  : 'careers_salesforce_after_skill_created',
                  'Payload':[
                    {'item1':'value1'},
                    {'item2':'value2'}
                  ],
                  'Timestamp': '2020-05-22T21:28:06.496Z',
                  'ExtraParams': { 'name': 'value'}
                }
            ";


            JObject json = JObject.Parse(responseBody);

            var contextValues = new Dictionary<string, IEnumerable<string>> { {"context1", new List<string>() { "1", "2", "3"} }, {"context2", new List<string>() { "1", "2", "3"} } };

            Event ev = new Event()
            {
                Name = (string)json["Name"],
                Timestamp = (DateTime)json["Timestamp"],
                Payload = json["Payload"].ToString(),
                ExtraParams = json["ExtraParams"].ToObject<JObject>(),
                ContextHeaders = contextValues
            };



            EventDispatcher.Dispatch(ev);



            Console.ReadKey();
        }

        private static HttpResponseMessage OnEvent1(Event e)
        {
            Log.Debug("---- Call from RegisterLocal callback");
            return null;
        }

    }
}
