using EventManager.BusinessLogic.Entities;
using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Factories;
using EventManager.BusinessLogic.Interfaces;
using EventManager.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManager.Middleware
{
    public class EventManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EventManagerConfiguration _config;
        private readonly EventDispatcher EventDispatcher;

        public EventManagerMiddleware(
                RequestDelegate next,
                IOptions<EventManagerConfiguration> config
            )
        {
            _next = next;
            _config = config.Value;
            EventManagerConstants.EventReceptionPath = !string.IsNullOrEmpty(_config.EventReceptionPath) ? _config.EventReceptionPath : EventManagerConstants.EventReceptionPath;
            EventManagerConstants.ReplyEventPrefix = !string.IsNullOrEmpty(_config.ReplyEventPrefix) ? _config.ReplyEventPrefix : EventManagerConstants.ReplyEventPrefix;
            EventDispatcher = EventDispatcher.Instance;

            foreach (SubscriptionConfiguration subscriptionConf in _config.Subscriptions)
            {
                foreach (EventSubscriberConfiguration eventSubscriberConf in subscriptionConf.Subscribers)
                {
                    ExternalServiceConfiguration externalService = _config.ExternalServices.Find(x => x.Name == eventSubscriberConf.Name);

                    if (externalService != null)
                    {
                        IAuthHandler auth = AuthFactory.Create(externalService.Auth);

                        if (!auth.Valid(externalService.Config, eventSubscriberConf))
                        {
                            throw new ArgumentException($"EventManagerMiddleware ERROR: externalService is not Valid for the externalService.Auth.Type `{externalService.Auth.Type}` and name `{externalService.Name}`, so it wont be registered with EventDispatcher.Register");
                        }
                        else
                        {
                            List<Action<Event>> callbacks = new List<Action<Event>>();

                            Subscriber subscriber = new Subscriber()
                            {
                                Config = new SubscriberConfig
                                {
                                    MaxTries = externalService.Config.MaxRetries,
                                    RequestRate = externalService.Config.RequestRate
                                }
                            };

                            eventSubscriberConf.Endpoint = AuthFactory.Endpoint(eventSubscriberConf, externalService);

                            Subscription subscription = new Subscription()
                            {
                                Subscriber = subscriber,
                                EventName = subscriptionConf.EventName,
                                Method = new HttpMethod(eventSubscriberConf.Method),
                                EndPoint = eventSubscriberConf.Endpoint,
                                CallBacks = callbacks,
                                IsExternal = true,
                                Auth = auth
                            };

                            EventDispatcher.Register(subscription);
                        }

                    }
                }
            }
        }

        // IMyScopedService is injected into Invoke
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string method = httpContext.Request.Method.ToLower();
            string path = httpContext.Request.Path;
            int statusCode = httpContext.Response.StatusCode;

            if (("post" == method) && (path == EventManagerConstants.EventReceptionPath))
            {
                string responseBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();

                JObject json = JObject.Parse(responseBody);

                Event e = new Event()
                {
                    Name = (string)json["Name"],
                    Timestamp = (DateTime)json["Timestamp"],
                    Payload = json["Payload"].ToString(Formatting.None),
                    ExtraParams = json["ExtraParams"].ToObject<JObject>(),
                };

                EventDispatcher.Dispatch(e);

                httpContext.Response.StatusCode = 200;
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(httpContext);
            }

        }

        // public async Task InvokeAsync(HttpContext context)
        // {
        //     var cultureQuery = context.Request.Query["culture"];
        //     if (!string.IsNullOrWhiteSpace(cultureQuery))
        //     {
        //         var culture = new CultureInfo(cultureQuery);

        //         CultureInfo.CurrentCulture = culture;
        //         CultureInfo.CurrentUICulture = culture;

        //     }

        //     // Call the next delegate/middleware in the pipeline
        //     await _next(context);
        // }
    }
}
