using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Extensions;
using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities.Auth
{
    class NoneAuth : IAuthHandler
    {
        readonly AuthConfig authConfig;
        public NoneAuth(AuthConfig authConfig)
        {
            this.authConfig = authConfig;
        }
        public async Task<HttpResponseMessage> SendEvent(Event e, Subscription s)
        {
            Log.Debug("NoneAuth.SendEvent");

            return await HttpClientExtension.MakeCallRequest(e.Payload, s.Method, s.EndPoint);
        }

        public bool Valid(Config config, EventSubscriberConfiguration eventSubscriberConfiguration)
        {
            Enum.TryParse(authConfig.Type, out AuthType authType);

            List<string> requiredProperties = new List<string>
            {
                config.BaseURL,
                eventSubscriberConfiguration.Endpoint,
                eventSubscriberConfiguration.Method
            };

            bool notEmpty = requiredProperties.All(property => !string.IsNullOrEmpty(property));

            return notEmpty && authType == AuthType.None;
        }
    }
}
