using EventManager.BusinessLogic.Entities.Config;
using EventManager.BusinessLogic.Interfaces;
using Serilog;
using System;
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
        private string User;
        private string Password;

        public BasicAuth(AuthConfig authConfig)
        {
            User = authConfig.User;
            Password = authConfig.Password;
        }
        public async Task<bool> SendEvent(Event e, Subscription subscription)
        {
            Log.Debug("BasicAuth.SendEvent");

            HttpClient _client = new HttpClient();

            string credentials = Convert.ToBase64String(Encoding.Default.GetBytes($"{User}:{Password}"));

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = subscription.Method,
                RequestUri = new Uri(subscription.EndPoint),
                Headers = { { "Authorization", $"Basic {credentials}" } }
            };

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(TypeJson));
            request.Content = new StringContent(e.Payload, Encoding.UTF8, TypeJson);

            var result = await _client.SendAsync(request, CancellationToken.None);

            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
