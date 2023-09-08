using Microled.Pix.Domain.Request.Bradesco;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microled.Pix.Infra.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Microled.Pix.Infra.Helpers
{
    public class PixItauHelpers : BaseHttpClient, IPixItauHelper
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PixItauHelpers(IConfiguration configuration)
        {
            _configuration = configuration;

            var handler = new HttpClientHandler();
            var certPath = @"C:\dev\microled\BRADESCO\certificados\BANDEIRANTES_DEICMAR_LOGISTICA_INTEGRADA_S_A_58188756000196_1677184234793938700.pfx";
            var certificate = new X509Certificate2(certPath, "12345678");
            handler.ClientCertificates.Add(certificate);

            _httpClient = new HttpClient(handler);
        }
        public async Task<string> GetAuthenticationToken(BankCredentials credentials)
        {
            string url = _configuration.GetSection("UrlsPixItau:authentication").Value;

            try
            {
                // Construir o header de autorização
                var authInfo = $"{credentials.ClientId}:{credentials.ClientSecret}";
                var authInfoBytes = Encoding.Default.GetBytes(authInfo);
                var authInfoBase64 = Convert.ToBase64String(authInfoBytes);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfoBase64);

                // Construir o corpo da requisição
                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                });

                var response = await _httpClient.PostAsync(url, requestData);

                // Se o código de status for bem-sucedido, retorne o corpo da resposta.
                // Caso contrário, lance uma exceção.
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Você pode incluir aqui mais lógica específica baseada em diferentes códigos de status se necessário.
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<ServiceResult<PagamentoResponse>> UpdateCobvEmvData(string token, string txid, RequestDataBradesco requestData)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            string url = _configuration.GetSection("UrlsPixItau:homolog_url").Value + "/v2/cobv-emv/" + txid;

            try
            {
                // Adicionar o token ao header de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Convertendo o objeto de requestData para JSON
                var jsonString = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                PagamentoResponse responseData = new PagamentoResponse();
                if (responseBody != null)
                {
                    _serviceResult.Result = responseData;
                }


                //validar o retorno e tratar para enviar no pagamento response

                return _serviceResult;
            }
            catch (Exception ex)
            {
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }

        }
    }
}
