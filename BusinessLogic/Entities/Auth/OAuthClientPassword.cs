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
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Entities.Auth
{
    public class OAuthClientPassword : IAuthHandler
    {
        public static string TypeJson = "application/json";
        private readonly AuthConfig authConfig;
        private readonly string LoginEndpoint;
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly string Username;
        private readonly string Password;

        // Token cache
        private string AccessToken;

        /// <summary>
        /// see https://tools.ietf.org/html/rfc6749#section-4.3
        /// </summary>
        /// <param name="authConfig"></param>
        public OAuthClientPassword(AuthConfig authConfig)
        {
            this.authConfig = authConfig;
            LoginEndpoint = authConfig.LoginEndpoint;
            ClientId = authConfig.ClientId;
            ClientSecret = authConfig.ClientSecret;
            Username = authConfig.Username;
            Password = authConfig.Password;
        }
        public async Task<HttpResponseMessage> SendEvent(Event e, Subscription subscription)
        {
            Log.Debug("OAuthClientPassword.SendEvent");

            HttpResponseMessage httpResponseMessage;
            HttpResponseMessage response;
            string jsonResponse;

            // obtain access token only if necessary
            if (AccessToken == null)
            {
                Log.Debug($"OAuthClientPassword.SendEvent: Obtaining access token for {subscription.Subscriber.Name}");
                using (var client = new HttpClient())
                {
                    var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            {"grant_type", "password"},
                            {"client_id", ClientId},
                            {"client_secret", ClientSecret},
                            {"username", Username},
                            {"password", Password}
                        }
                    );
                    response = await client.PostAsync(LoginEndpoint, encodedContent);
                    jsonResponse = response.Content.ReadAsStringAsync().Result;
                }

                Log.Debug($"OAuthClientPassword.SendEvent, Credentials response: {jsonResponse}");

                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                if (values.ContainsKey("error"))
                {
                    // fail if there was an error
                    httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("error", Encoding.UTF8, "application/json")
                    };
                    return httpResponseMessage;
                }

                // otherwise set access token property
                AccessToken = values["access_token"];
            }

            Log.Debug($"OAuthClientPassword.SendEvent, Using access_token: {AccessToken}");

            // build request
            HttpRequestMessage request = new HttpRequestMessage(subscription.Method, subscription.EndPoint);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(TypeJson));
            request.Content = new StringContent(e.Payload, Encoding.UTF8, TypeJson);

            // send request
            using (var client = new HttpClient())
            {
                Log.Debug($"OAuthClientPassword.SendEvent, Sending request for '{subscription.Subscriber.Name}' to {subscription.Method} {subscription.EndPoint}");
                httpResponseMessage = await client.SendAsync(request);
            }

            Log.Debug($"OAuthClientPassword.SendEvent, StatusCode:  {httpResponseMessage.StatusCode}");

            string responseResult = await httpResponseMessage.Content.ReadAsStringAsync();

            // Check for errors: EM should trigger a retry if the response is invalid
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Log.Debug($"OAuthClientPassword.SendEvent, ERROR : BadRequest: " + responseResult);
                return httpResponseMessage;
            }
            else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // According to https://tools.ietf.org/id/draft-ietf-oauth-v2-bearer-09.xml the server should return a
                // HTTP 401 (Unauthorized) status code if the access token has expired.

                // setting the token to Null will mean that it will be re-obtained on the next request
                this.AccessToken = null;
                return httpResponseMessage;
            }

            Log.Debug($"OAuthClientPassword.SendEvent, Response: {responseResult}");

            return httpResponseMessage;
        }


        public bool Valid(Config config, EventSubscriberConfiguration eventSubscriberConfiguration)
        {
            Enum.TryParse(authConfig.Type, out AuthType authType);

            List<string> requiredProperties = new List<string>
            {
                authConfig.LoginEndpoint,
                authConfig.ClientId,
                authConfig.ClientSecret,
                authConfig.Username,
                authConfig.Password,
                eventSubscriberConfiguration.Endpoint,
                eventSubscriberConfiguration.Method
            };

            bool notEmpty = requiredProperties.All(property => !string.IsNullOrEmpty(property));

            return notEmpty && authType == AuthType.OAuthClientPassword;
        }
    }
}
