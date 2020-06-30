using EventManager.BusinessLogic.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities
{
    public class Subscription
    {
        public string EventName { get; set; }
        public Subscriber Subscriber { get; set; }
        public HttpMethod Method { get; set; }
        public string EndPoint { get; set; }
        public List<Action<Event>> CallBacks { get; set; }
        public bool IsExternal { get; set; }

        public Subscription()
        {
        }

        /// <summary>
        /// SendEvent function
        /// </summary>
        /// <param name="_event">Event object</param>
        /// <returns>Boolean</returns>
        public async Task<bool> SendEvent(Event _event)
        {
            var json = _event.Payload;

            if (this.IsExternal)
            {
                Log.Debug("Subscription.SendEvent: isExternal true");
                return await HttpClientExtension.MakeCallRequest(json, this.Method, this.EndPoint);
            }
            else
            {
                Log.Debug("Subscription.SendEvent: isExternal false");
                try
                {
                    foreach (var callback in CallBacks)
                    {
                        callback.Invoke(_event);
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
