using EventManager.BusinessLogic.Entities;
using EventManager.Data;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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
        /// <returns>HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> MakeCallRequest(string body, HttpMethod method, string endpoint)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, endpoint);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(TypeJson));
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(body, Encoding.UTF8, TypeJson);

            HttpResponseMessage httpResponseMessage = await _client.SendAsync(request, CancellationToken.None);

            return httpResponseMessage;
        }
    }
}
