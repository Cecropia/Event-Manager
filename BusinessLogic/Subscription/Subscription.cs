using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Subscription
{
    public class Subscription
    {
        public string EventName { get; set; }
        public Subscriber Subscriber { get; set; }
        public string Method { get; set; }
        public string EndPoint { get; set; }
        public List<Action<Event>> CallBacks { get; set; }
        public bool IsExternal { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventName">Event Name</param>
        /// <param name="subscriber">Subscriber object</param>
        /// <param name="method">Method</param>
        /// <param name="endPoint">URL</param>
        /// <param name="callBacks">List callbacks</param>
        /// <param name="isExternal">Boolean</param>
        public Subscription(string eventName, Subscriber subscriber, string method, string endPoint, List<Action<Event>> callBacks, bool isExternal)
        {
            this.EventName = eventName;
            this.Subscriber = subscriber;
            this.Method = method;
            this.EndPoint = endPoint;
            this.CallBacks = callBacks;
            this.IsExternal = isExternal;
        }

        /// <summary>
        /// SendEvent function
        /// </summary>
        /// <param name="_event">Event object</param>
        /// <returns>Boolean</returns>
        public async Task<bool> SendEvent(Event _event)
        {
            var json = JsonConvert.SerializeObject(_event.Payload);
            if (this.IsExternal)
            {
                return await HttpClientExtension.MakeCallRequest(json, this.Method, this.EndPoint);
            }
            else
            {
                try
                {
                    foreach (var f in this.CallBacks)
                    {
                        f.Invoke(_event);
                        //TODO Handle async actions
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                
            }

        }

    }
}
