using EventManager.Data;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace EventManager.BusinessLogic.Entities
{
    /// <summary>
    /// Class to handle a list of <see cref="QueueItem"/>
    /// to be processed.
    /// </summary>
    public sealed class Queue
    {
        private static readonly Lazy<Queue> lazy = new Lazy<Queue>(() => new Queue());
        public static Queue Instance { get { return lazy.Value; } }
        public readonly ConcurrentQueue<QueueItem> Items;
        private static Timer timer = null;
        private static int running = 0;
        private readonly static int milliseconds = 1;


        /// <summary>
        /// Queue Constructor, only invoked through Instance
        /// </summary>
        private Queue()
        {
            Log.Debug("Queue.Queue");
            Items = new ConcurrentQueue<QueueItem>();

            if (timer == null)
            {
                running = 1;
                timer = new Timer(new TimerCallback(Tick), null, 1, milliseconds);
            }
        }

        /// <summary>
        /// Callback for the timer (it runs on a Threadpool)
        /// </summary>
        /// <param name="timerState"></param>
        private static void Tick(object timerState)
        {
            Log.Debug("Queue.Tick");
            ThreadPool.QueueUserWorkItem(Instance.Process);
        }

        /// <summary>
        /// Add <see cref="QueueItem"/> to the queue list.
        /// </summary>
        /// <param name="item">QueueItem to be processed</param>
        public void Add(QueueItem item)
        {
            Log.Debug("Queue.Add: " + item.Guid.ToString());
            Items.Enqueue(item);
            if (Interlocked.Equals(running, 0)) //!running
            {
                Log.Debug("Queue.Add: Timer restarted");
                Interlocked.Exchange(ref running, 1);
                timer.Change(1, milliseconds);
            }
        }

        /// <summary>
        /// Takes a QueueItem to be processed and then processes it.
        /// If there are no more QueueItems then the Timer can be suspended.
        /// </summary>
        /// <param name="stateInfo"></param>
        public void Process(Object stateInfo)
        {
            Log.Debug("Queue.Process");
            Log.Debug("Queue.Process: Count: " + Items.Count.ToString());
            QueueItem itemToProcess;
            if (Items.TryDequeue(out itemToProcess))
            {
                ProcessItem(itemToProcess);
            }
            else
            {
                if (Interlocked.Equals(running, 1))
                {
                    Log.Debug("Queue.Process: Suspending Timer.");
                    Interlocked.Exchange(ref running, 0); //running = false;
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        private async void ProcessItem(QueueItem item)
        {
            Log.Debug("Queue.ProcessItem, QueueItem.Guid: " + item.Guid.ToString());
            // send a HTTP request to the endpoint specified in the descripcion
            // The sending of the element should be done through the subscription object associated with the event

            SubscriberConfig subscriberConfig = item.Subscription.Subscriber.Config;
            int millisecondRate = 1000 / subscriberConfig.RequestRate;
            int timeDiff = (DateTime.Now - item.LastTry).Milliseconds;

            if (timeDiff >= millisecondRate)
            {
                Log.Debug("Queue.ProcessItem: timeDiff: " + timeDiff);
                Log.Debug($"Queue.ProcessItem: item.Tries {item.Tries}");

                if (++item.Tries <= subscriberConfig.MaxTries)
                {
                    item.LastTry = DateTime.Now;
                    HttpResponseMessage httpResponseMessage = await item.Subscription.SendEvent(item.Event);

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        item.Status = HttpStatusCode.OK;
                        Log.Debug("Queue.ProcessItem, Item Processed Status OK: " + item.Guid.ToString());

                        /// Dispatch a NEW Event with the response of the current Event
                        string contents = await httpResponseMessage.Content.ReadAsStringAsync();
                        DispatchResponseEvent(contents, item.Event.Name);
                        /////////////////////////////////

                        // If after processing items there are still items in queue
                        // then process the next one
                        ThreadPool.QueueUserWorkItem(Instance.Process);
                    }
                    else
                    {
                        // If the SendEvent fails (not OK) then sent back to the queue for it to be processed again
                        Log.Debug("Queue.ProcessItem, Item Processed FAILED, back to queue " + item.Guid.ToString());
                        Items.Enqueue(item);

                        if (Interlocked.Equals(running, 0)) //!running
                        {
                            Log.Debug("Queue.ProcessItem: Timer restarted!!");
                            Interlocked.Exchange(ref running, 1);
                            timer.Change(1, milliseconds);
                        }
                    }
                }
                else
                {
                    // In the future here should exist a way to notify back
                    // that the QueueItem has reached its max tries
                    Log.Debug("Queue.ProcessItem: Error: MaxTries Reached");
                }
            }
            else
            {
                Log.Debug("Queue.ProcessItem: Back to Queue: Request Rate too soon.");
                Items.Enqueue(item);
            }
        }

        private void DispatchResponseEvent(string payload, string eventName)
        {
            Event ev = new Event()
            {
                Name = EventManagerConstants.ReplyEventPrefix + eventName,
                Timestamp = DateTime.UtcNow,
                Payload = payload,
                ExtraParams = null,
            };

            EventDispatcher dispatcher = EventDispatcher.Instance;
            dispatcher.Dispatch(ev);
        }
    }
}
