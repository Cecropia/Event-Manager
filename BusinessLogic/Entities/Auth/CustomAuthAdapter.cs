using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities.Auth
{
    public class CustomAuthAdapter : IAuthHandler
    {
        public static string TypeJson = "application/json";
        private readonly AuthConfig authConfig;

        public CustomAuthAdapter(AuthConfig authConfig)
        {
            this.authConfig = authConfig;
        }
        public async Task<HttpResponseMessage> SendEvent(Event e, Subscription subscription)
        {

            Log.Debug("CustomAuthAdapter.SendEvent");

            HttpClient _client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = subscription.Method,
                RequestUri = new Uri(subscription.EndPoint)
            };

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(TypeJson));
            request.Content = new StringContent(e.Payload, Encoding.UTF8, TypeJson);

            // just before sending the request we pass it to the custom auth provider with the specified name, if any
            var customProvider = EventDispatcher.Instance.customAuthProviderRegistry.GetAuthProvider(
              this.authConfig.CustomAuthProviderName
            );

            if (customProvider == null)
            {
                // TODO handle this error
                throw new ApplicationException($"Expected a custom auth provider with name '{this.authConfig.CustomAuthProviderName}' but none was found in the register");
            }

            // allow the provider to modify the request as it sees fit
            customProvider(request);

            HttpResponseMessage httpResponseMessage = await _client.SendAsync(request, CancellationToken.None);
            return httpResponseMessage;
        }


        /// <summary>
        /// Checks for the required properties to be present
        /// </summary>
        /// <param name="config"></param>
        /// <param name="eventSubscriberConfiguration"></param>
        /// <returns>Bool indicating if is valid or not</returns>
        public bool Valid(Config config, EventSubscriberConfiguration eventSubscriberConfiguration)
        {
            Enum.TryParse(authConfig.Type, out AuthType authType);

            List<string> requiredProperties = new List<string>
            {
                config.BaseURL,
                eventSubscriberConfiguration.Endpoint,
                eventSubscriberConfiguration.Method,
                // TODO: this doesn't really work because `Valid` gets called before the custom auth provider is instantiated
                // EventDispatcher.Instance.customAuthProvider.GetAuthProvider(this.authConfig.CustomAuthProviderName)?.ToString()
            };

            bool notEmpty = requiredProperties.All(property => !string.IsNullOrEmpty(property));

            return notEmpty && authType == AuthType.CustomAuth;
        }
    }
}
