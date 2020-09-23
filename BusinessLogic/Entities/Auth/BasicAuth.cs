using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace EventManager.BusinessLogic.Entities.Auth
{
    public class BasicAuth : IAuthHandler
    {
        public static string TypeJson = "application/json";
        private readonly AuthConfig authConfig;
        private readonly string Username;
        private readonly string Password;

        public BasicAuth(AuthConfig authConfig)
        {
            this.authConfig = authConfig;
            Username = authConfig.Username;
            Password = authConfig.Password;
        }
        public async Task<HttpResponseMessage> SendEvent(Event e, Subscription subscription)
        {
            Log.Debug("BasicAuth.SendEvent");

            HttpClient _client = new HttpClient();

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = subscription.Method,
                RequestUri = new Uri(subscription.EndPoint),
                Headers = { { "Authorization", $"Basic {credentials}" } }
            };

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(TypeJson));
            request.Content = new StringContent(e.Payload, Encoding.UTF8, TypeJson);

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
                authConfig.Username,
                authConfig.Password,
                eventSubscriberConfiguration.Endpoint,
                eventSubscriberConfiguration.Method
            };

            bool notEmpty = requiredProperties.All(property => !string.IsNullOrEmpty(property));

            return notEmpty && authType == AuthType.Basic;
        }
    }
}
