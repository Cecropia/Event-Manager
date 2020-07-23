using EventManager.BusinessLogic.Entities.Configuration;
using EventManager.BusinessLogic.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string Token;

        // Token cache
        private string AccessToken;
        private string InstanceUrl;


        public OAuthClientPassword(AuthConfig authConfig)
        {
            this.authConfig = authConfig;
            LoginEndpoint = authConfig.LoginEndpoint;
            ClientId = authConfig.ClientId;
            ClientSecret = authConfig.ClientSecret;
            Username = authConfig.Username;
            Password = authConfig.Password;
            Token = authConfig.Token;
        }
        public async Task<bool> SendEvent(Event e, Subscription subscription)
        {
            Log.Debug("BasicAuth.SendEvent");

            HttpResponseMessage response;
            string jsonResponse;

            if (AccessToken == null)
            {
                // login
                Log.Debug("OAuthClientPassword.SendEvent: login");
                using (var client = new HttpClient())
                {
                    var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            {"grant_type", "password"},
                            {"client_id", ClientId},
                            {"client_secret", ClientSecret},
                            {"username", Username},
                            {"password", Password + Token}
                        }
                    );
                    response = await client.PostAsync(LoginEndpoint, encodedContent);
                    jsonResponse = response.Content.ReadAsStringAsync().Result;
                }
                Log.Debug($"OAuthClientPassword.SendEvent, Response: {jsonResponse}");

                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                if (values.ContainsKey("error"))
                {
                    return false;
                }
                else
                {
                    AccessToken = values["access_token"];
                    InstanceUrl = values["instance_url"];
                }
            }

            Log.Debug("OAuthClientPassword.SendEvent: No Login");
            Log.Debug($"OAuthClientPassword.SendEvent, AccessToken: {AccessToken}");
            Log.Debug($"OAuthClientPassword.SendEvent, InstanceUrl: {InstanceUrl}");

            HttpClient _client = new HttpClient();

            string restRequest = InstanceUrl + subscription.EndPoint;
            HttpRequestMessage request = new HttpRequestMessage(subscription.Method, restRequest);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(TypeJson));
            request.Content = new StringContent(e.Payload, Encoding.UTF8, TypeJson);

            HttpResponseMessage result = await _client.SendAsync(request);

            Log.Debug($"OAuthClientPassword.SendEvent, StatusCode:  {result.StatusCode}");


            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Log.Debug($"OAuthClientPassword.SendEvent, ERROR : BadRequest: " + result.Content.ReadAsStringAsync().Result);
                return false;
            }

            Log.Debug($"OAuthClientPassword.SendEvent, Result:{result.Content.ReadAsStringAsync().Result}");


            ResetAccessTokenIfErrorInResponseMessage(result);

            // TODO: add helper, to dispatch the response and use the same call in other Auths

            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Takes an HttpResponseMessage, verifies if it has an INVALID_SESSION_ID error
        /// if so, then sets the AccesToken to null, so it can log in again
        /// </summary>
        /// <param name="response"></param>
        private void ResetAccessTokenIfErrorInResponseMessage(HttpResponseMessage response)
        {
            List<Dictionary<string, string>> resultValues = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response.Content.ReadAsStringAsync().Result);

            if (resultValues.Count > 0)
            {
                Dictionary<string, string> firstItem = resultValues[0];
                if (firstItem.ContainsKey("errorCode") && firstItem["errorCode"] == "INVALID_SESSION_ID")
                {
                    Log.Debug($"OAuthClientPassword.ResetAccessTokenIfErrorInResponseMessage: AccessToken set to `null` ");
                    AccessToken = null;
                }
            }
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
                authConfig.LoginEndpoint,
                authConfig.ClientId,
                authConfig.ClientSecret,
                authConfig.Username,
                authConfig.Password,
                authConfig.Token,
                eventSubscriberConfiguration.Endpoint,
                eventSubscriberConfiguration.Method
            };

            bool notEmpty = requiredProperties.All(property => !string.IsNullOrEmpty(property));

            return notEmpty && authType == AuthType.OAuthClientPassword;
        }
    }
}