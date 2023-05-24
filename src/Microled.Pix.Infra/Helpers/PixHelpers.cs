using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microled.Pix.Infra.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Microled.Pix.Infra.Helpers
{
    public class PixHelpers : BaseHttpClient, IPixHelper
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PixHelpers(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }
        public async Task<string> getAuthenticationToken(BankCredentials credentials)
        {
            //realizar a requisicao para auth02
            string url = _configuration.GetSection("UrlsPix:devportal_url").Value + _configuration.GetSection("UrlsPix:authentication").Value;

            var requestData = new
            {
                grant_type = "client_credentials",
                client_id = credentials.ClientId,
                client_secret = credentials.ClientSecret
            };

            string _requestData = JsonSerializer.Serialize(requestData);

            var response = await SendAsync(HttpMethod.Post, url, _requestData);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            // Parse o responseBody para extrair o token de autenticação

            return responseBody;
        }

        public void VerificarPagamentosPix()
        {
            //chamar api microled passando CHAVE / TXID
            throw new NotImplementedException();
        }
    }
}
