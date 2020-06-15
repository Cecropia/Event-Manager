using EventManager.BusinessLogic.Entities;
using EventManager.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;


namespace ExecutableTest
{
    class Program
    {
        static Queue queue;
        static EventDispatcher EventDispatcher;

        static void Main(string[] args)
        {
            //queue_test();
            event_dispatcher_test();
        }

        public static void queue_test()
        {
            Console.WriteLine("---- Queue Test");


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

            Event ev = new Event()
            {
                Name = (string)json["Name"],
                Timestamp = (DateTime)json["Timestamp"],
                Payload = json["Payload"] as JObject,
                ExtraParams = json["ExtraParams"] as JObject,
            };

            List<Action<Event>> callbacks = new List<Action<Event>>();

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
                EventName = "event_name",
                Method = "POST",
                EndPoint = EventManagerConstants.EventReceptionPath,
                CallBacks = callbacks
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

            List<Action<Event>> callbacks = new List<Action<Event>>();

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
                EventName = "careers_salesforce_after_skill_created",
                Method = "POST",
                EndPoint = EventManagerConstants.EventReceptionPath,
                CallBacks = callbacks
            };

            EventDispatcher.Register(subscription);


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
                  'ExtraParams': { 'name': 'value'}, 
                }
            ";


            JObject json = JObject.Parse(responseBody);

            Event ev = new Event()
            {
                Name = (string)json["Name"],
                Timestamp = (DateTime)json["Timestamp"],
                Payload = json["Payload"] as JObject,
                ExtraParams = json["ExtraParams"] as JObject,
            };



            EventDispatcher.Dispatch(ev);



            Console.ReadKey();
        }

    }
}
