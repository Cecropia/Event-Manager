using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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

        public IAuthHandler Auth { get; set; }

        public Subscription()
        {
        }

        /// <summary>
        /// SendEvent function
        /// </summary>
        /// <param name="_event">Event object</param>
        /// <returns>Boolean</returns>
        public async Task<HttpResponseMessage> SendEvent(Event _event)
        {
            HttpResponseMessage httpResponseMessage;

            if (this.IsExternal)
            {
                Log.Debug("Subscription.SendEvent: isExternal true");
                return await this.Auth.SendEvent(_event, this);
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
                    string SerializedString = "true";
                    httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SerializedString, Encoding.UTF8, "application/json") };
                    return httpResponseMessage;
                }
                catch (Exception)
                {
                    string SerializedString = "false";
                    httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(SerializedString, Encoding.UTF8, "application/json") };
                    return httpResponseMessage;
                }
            }
        }
    }
}
