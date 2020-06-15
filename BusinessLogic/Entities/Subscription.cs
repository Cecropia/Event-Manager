using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities
{
    public class Subscription
    {
        public string EventName { get; set; }
        public Subscriber Subscriber { get; set; }
        public string Method { get; set; }
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
