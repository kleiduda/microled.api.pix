using System.Text;
using System.Net.Http;

namespace Microled.Pix.Infra.Http
{
    public abstract class BaseHttpClient
    {
        private readonly HttpClient _httpClient;

        protected BaseHttpClient()
        {
            _httpClient = new HttpClient();
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string jsonBody = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (!string.IsNullOrEmpty(jsonBody))
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                request.Content = content;
            }

            var response = await _httpClient.SendAsync(request);
            return response;
        }
    }
}
