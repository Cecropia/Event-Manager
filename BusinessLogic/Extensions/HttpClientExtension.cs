using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManager.BusinessLogic.Extensions
{
    /// <summary>
    /// HttpClient Request static class
    /// </summary>
    public static class HttpClientExtension
    {
        private static readonly HttpClient _client = new HttpClient();
        public static string TypeJson = "application/json";

        /// <summary>
        /// Make Request Function
        /// </summary>
        /// <param name="body">Payload</param>
        /// <param name="method">Method(POST/PUT)</param>
        /// <param name="endpoint">URL</param>
        /// <returns>Boolean</returns>
        public static async Task<bool> MakeCallRequest(string body, string method, string endpoint)
        {
            method = method.ToLower();
            _client.BaseAddress = new Uri(endpoint);
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var result = (method) =="post" ? await _client.PostAsync(endpoint, content) : await _client.PutAsync(endpoint, content);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
