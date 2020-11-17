using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace EventManager.BusinessLogic.Entities
{
    public class Subscription
    {
        public string EventName { get; set; }
        public Subscriber Subscriber { get; set; }
        public HttpMethod Method { get; set; }
        public string EndPoint { get; set; }
        public List<Func<Event, HttpResponseMessage>> CallBacks { get; set; }
        public bool IsExternal { get; set; }
        public bool Synchronous { get; set; }

        public IAuthHandler Auth { get; set; }

        public Subscription()
        {
        }

        /// <summary>
        /// This method can be used to replace the templateValues in the provided "str". Each template value in str 
        /// should have the shape "{templateValueName}". This method will find all instances of said templateValues
        /// and look for "templateValueName" in the <paramref name="templateValues"/> dictionary. Note that if no templateValue
        /// is found for a key then this is an error and it will throw a <see cref="System.Collections.Generic.KeyNotFoundException"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="templateValues"></param>
        /// <returns>A new string with all the template values replaced</returns>
        private string ApplyTemplateValuesToString(string str, Dictionary<string, string> templateValues)
        {
            Log.Debug("Subscription.ApplyTemplateValuesToUri: applying string template replacement to endpoint");

            var rx = new Regex(@"\{.*?\}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var matches = rx.Matches(str);
            foreach (Match m in matches)
            {
                Log.Debug($"Subscription.ApplyTemplateValuesToUri: replacing match for {m.Value}");

                var val = m.Value;
                var key = val.Replace("{", "").Replace("}", "");

                // key should exist, otherwise this is an error
                str = str.Replace(
                    m.Value,
                    HttpUtility.UrlEncode(templateValues[key])
                );
            }

            return str;
        }

        /// <summary>
        /// SendEvent function
        /// </summary>
        /// <param name="_event">Event object</param>
        /// <returns>Boolean</returns>
        public async Task<HttpResponseMessage> SendEvent(Event _event)
        {
            HttpResponseMessage httpResponseMessage;

            // if the event `isExternal` then send it to the external recipient, otherwise
            // route it to the appropriate internal handler
            if (this.IsExternal)
            {
                Log.Debug("Subscription.SendEvent: isExternal true");

                var specificSubscription = this;

                // if it is external then also update the URL (if necessary)
                if (_event.UrlTemplateValues != null && _event.UrlTemplateValues.Count > 0)
                {
                    // set specificSubscription to be a clone of `this` so that we don't affect `this`'s
                    // values
                    specificSubscription = (Subscription)this.MemberwiseClone();

                    specificSubscription.EndPoint = this.ApplyTemplateValuesToString(
                        specificSubscription.EndPoint,
                        _event.UrlTemplateValues
                    );
                }

                return await this.Auth.SendEvent(_event, specificSubscription);
            }
            else
            {
                Log.Debug("Subscription.SendEvent: isExternal false");
                try
                {
                    foreach (var callback in CallBacks)
                    {
                        // if the subscription is synchronous then it should only have one callback
                        // so we're safe returning its response here
                        if (this.Synchronous)
                        {
                            return callback.Invoke(_event);
                        }

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
