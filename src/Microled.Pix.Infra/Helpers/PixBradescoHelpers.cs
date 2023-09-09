using Microled.Pix.Domain.Request.Bradesco;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.Response.Bradesco;
using Microled.Pix.Domain.Response.PixBradesco;
using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microled.Pix.Infra.Http;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microled.Pix.Infra.Helpers
{
    public class PixBradescoHelpers : BaseHttpClient, IPixBradescoHelper
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PixBradescoHelpers(IConfiguration configuration)
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
            string url = _configuration.GetSection("UrlsPixBradesco:authentication").Value ?? "";

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
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<ServiceResult<PagamentoResponse>> UpdateCobvEmvData(string token, RequestDataBradesco requestData)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            string _txId = CreateNewTxId();


            string url = _configuration.GetSection("UrlsPixBradesco:homolog_url").Value + "/v2/cobv-emv/" + _txId;

            try
            {
                // Adicionar o token ao header de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Convertendo o objeto de requestData para JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                var jsonString = JsonSerializer.Serialize(requestData, options);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody != null)
                {
                    BradescoPixResponse pixResponse = JsonSerializer.Deserialize<BradescoPixResponse>(responseBody);
                    PagamentoResponse pagamentoResponse = new PagamentoResponse()
                    {
                        IdEmpresa = 1,
                        Pagamento = pixResponse.cobv.txid,
                        Empresa = pixResponse.cobv.recebedor.nome,
                        Processo = 1,
                        StatusPagamento = pixResponse.cobv.status,
                        QRCode_Imagem_base64 = pixResponse.base64,
                        Pix_Link = pixResponse.emv,
                        QRCode_Texto_EMV = pixResponse.emv,
                        ValorRet = Convert.ToDecimal(pixResponse.cobv.valor.original),


                    };

                    _serviceResult.Result = pagamentoResponse;

                }
                return _serviceResult;
            }
            catch (Exception ex)
            {
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }

        }

        public async Task<ServiceResult<PagamentoResponse>> ConsultarQrCodePix(string token, string txId)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            //
            string url = _configuration.GetSection("UrlsPixBradesco:homolog_url").Value + "/v2/cobv/" + txId;
            try
            {
                // Adicionar o token ao header de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // var content = new StringContent(Encoding.UTF8, "application/json");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody != null)
                {
                    ConsultaQrCodeResponse pixResponse = JsonSerializer.Deserialize<ConsultaQrCodeResponse>(responseBody);
                    PagamentoResponse pagamentoResponse = new PagamentoResponse()
                    {
                        IdEmpresa = 1,
                        Pagamento = pixResponse.txid,
                        Empresa = pixResponse.recebedor.nome,
                        Processo = 1,
                        StatusPagamento = pixResponse.status,
                        QRCode_Imagem_base64 = "",
                        Pix_Link = pixResponse.pixCopiaECola,
                        QRCode_Texto_EMV = pixResponse.pixCopiaECola,
                        ValorRet = Convert.ToDecimal(pixResponse.valor.original)

                    };

                    _serviceResult.Result = pagamentoResponse;

                }
            }
            catch (Exception ex)
            {
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }

            return _serviceResult;
        }

        private string CreateNewTxId()
        {
            Guid guid = Guid.NewGuid();
            string txId = guid.ToString("N");

            if (txId.Length > 35)
            {
                txId = txId.Substring(0, 35);
            }

            return txId;
        }

    }
}
