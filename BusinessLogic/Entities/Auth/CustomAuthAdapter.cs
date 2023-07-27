using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Entities.Exceptions;
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
    /// <summary>
    /// This type of authorization allows the user of the library to specify an `Action`
    /// that is invoked once the <see cref="System.Net.Http.HttpRequestMessage"/> is built. 
    /// This action is passed said request as well as the corresponding Subscription, 
    /// and is free to modify said request as it sees fit (eg, adding a header).
    /// </summary>
    public class CustomAuthAdapter : IAuthHandler
    {
        public static string TypeJson = "application/json";
        private readonly AuthConfig authConfig;

        public CustomAuthAdapter(AuthConfig authConfig)
        {
            this.authConfig = authConfig;
        }
        public async Task<HttpResponseMessage> SendEvent(Event e, Subscription subscription, List<KeyValuePair<string, string>> paramsList = null)
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
            var customProvider = EventDispatcher.customAuthProviderRegistry.GetAuthProvider(
              this.authConfig.CustomAuthProviderName
            );

            if (customProvider == null)
            {
                // TODO handle this error
                throw new KeyNotFoundException($"Expected a custom auth provider with name '{this.authConfig.CustomAuthProviderName}' but none was found in the register");
            }

            try
            {
                // allow the provider to modify the request and additional info as it sees fit
                customProvider(
                    request,
                    subscription
                );

                HttpResponseMessage httpResponseMessage = await _client.SendAsync(request, CancellationToken.None);
                return httpResponseMessage;

            }
            catch (CustomAuthFailureException ex)
            {
                // if there are any "controlled" errors during the custom auth then log it and return a 
                // dummy failed response
                Log.Error("CustomAuthAdapter.SendEvent - There was an error while executing the custom auth " +
                            "provider method: " + ex.Message + " - Stacktrace: " + ex.StackTrace);

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

        }


        public bool Valid(Config config, EventSubscriberConfiguration eventSubscriberConfiguration)
        {
            Enum.TryParse(authConfig.Type, out AuthType authType);

            List<string> requiredProperties = new List<string>
            {
                config.BaseURL,
                eventSubscriberConfiguration.Endpoint,
                eventSubscriberConfiguration.Method,
                // > the line below doesn't really work because `Valid` gets called before the custom auth provider is instantiated
                // EventDispatcher.Instance.customAuthProvider.GetAuthProvider(this.authConfig.CustomAuthProviderName)?.ToString()
            };

            bool notEmpty = requiredProperties.All(property => !string.IsNullOrEmpty(property));

            return notEmpty && authType == AuthType.CustomAuth;
        }
    }
}
