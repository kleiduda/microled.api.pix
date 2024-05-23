using Microled.Pix.Domain.Request.Bradesco;
using Microled.Pix.Domain.Request.Itau;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.Response.itau;
using Microled.Pix.Domain.Response.itau.lista;
using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microled.Pix.Infra.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.WebSockets;
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
            _httpClient = CreateHttpClient();
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            var certPath = _configuration["CredenciaisItau:certificado"];
            var keyPath = _configuration["CredenciaisItau:key"];
            var certPassword = _configuration["CredenciaisItau:password"];
            // var certPassword = Encoding.UTF8.GetString(Convert.FromBase64String(certPasswordBase64));


            try
            {

                if (File.Exists(certPath))
                {
                    var certificate = new X509Certificate2(certPath, certPassword);
                    handler.ClientCertificates.Add(certificate);
                }
                else
                {
                    throw new FileNotFoundException("O caminho especificado para o certificado não existe.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Falha ao carregar o certificado." + certPath + " - Pass: " + certPassword, ex);
            }

            return new HttpClient(handler);
        }

        public async Task<ServiceResult<string>> BaixaTitulo(string txId, string token)
        {
            ServiceResult<string> _serviceResult = new ServiceResult<string>();
            // Construindo a URL com parâmetros
            string baseURL = _configuration.GetSection("UrlsMicroled:band").Value;
            string url = $"{baseURL}/wspix.asmx/IntegrabaixaChronos?NumeroTitulo={txId}&Token={Uri.EscapeDataString(token)}";

            try
            {
                // Configurar o HttpClient para GET
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Ler e processar a resposta
                var responseBody = await response.Content.ReadAsStringAsync();
                //var responseData = JsonSerializer.Deserialize<PagamentoResponse>(responseBody);

                _serviceResult.Result = responseBody;


                return _serviceResult;
            }
            catch (Exception ex)
            {
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }
        }

        //public async Task<ServiceResult<PagamentoResponse>> BaixaTitulo(string txId, string token)
        //{
        //    ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
        //    string url = _configuration.GetSection("UrlsMicroled:baixa").Value;
        //    try
        //    {
        //        // Construir o corpo da requisição SOAP
        //        var soapString = $@"<?xml version=""1.0"" encoding=""utf-8""?>
        //    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
        //        xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
        //        xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
        //      <soap:Body>
        //        <BaixaTitulo xmlns=""http://tempuri.org/"">
        //          <txId>{txId}</txId>
        //          <token>{token}</token>
        //        </BaixaTitulo>
        //      </soap:Body>
        //    </soap:Envelope>";

        //        var content = new StringContent(soapString, Encoding.UTF8, "text/xml");
        //        _httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/BaixaTitulo");

        //        var response = await _httpClient.PostAsync(url, content);
        //        response.EnsureSuccessStatusCode();
        //        var responseBody = await response.Content.ReadAsStringAsync();

        //        // Processar a resposta SOAP aqui
        //        // O código para deserializar o XML vai depender do formato esperado da resposta.

        //        return _serviceResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        _serviceResult.Error = ex.Message;
        //        return _serviceResult;
        //    }
        //}

        public async Task<string> GetAuthenticationToken(BankCredentials credentials)
        {
            string url = _configuration.GetSection("UrlsPixItau:authentication").Value;

            try
            {
                // Construir o header de autorização
                // Configure os cabeçalhos adicionais necessários
                _httpClient.DefaultRequestHeaders.Add("x-itau-flowID", "1");
                _httpClient.DefaultRequestHeaders.Add("x-itau-correlationID", "2");

                // Construir o corpo da requisição com client_id e client_secret
                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", credentials.ClientId),
                    new KeyValuePair<string, string>("client_secret", credentials.ClientSecret)
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

        //public async Task<ServiceResult<PagamentoResponse>> NewCobPixQRCODE(string token, string txid, RequestDataItau requestData)
        //{
        //    ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
        //    string url = _configuration.GetSection("UrlsPixItau:qrcode_pix").Value + txid;
        //    //"/pix_recebimentos_conciliacoes/v2/post/cobrancas_vencimento_pix";

        //    // Carregar o certificado .pfx
        //    string certificatePath = @"C:\dev\ITAU\NEW\CERTIFICADO_PFX.pfx";
        //    string certificatePassword = "index@12";
        //    var certificate = new X509Certificate2(certificatePath, certificatePassword);

        //    // Configurar HttpClientHandler
        //    var handler = new HttpClientHandler();
        //    handler.ClientCertificates.Add(certificate);

        //    // Criar HttpClient com o handler customizado
        //    var httpClient = new HttpClient(handler);
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    try
        //    {
        //        var jsonString = JsonSerializer.Serialize(requestData);
        //        var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

        //        var response = await httpClient.PutAsync(url, content);
        //        response.EnsureSuccessStatusCode();

        //        var responseBody = await response.Content.ReadAsStringAsync();

        //        PagamentoResponse responseData = new PagamentoResponse();
        //        if (responseBody != null)
        //        {
        //            _serviceResult.Result = responseData;
        //        }

        //        return _serviceResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        _serviceResult.Error = ex.Message;
        //        return _serviceResult;
        //    }
        //    finally
        //    {
        //        httpClient.Dispose();  // Garantir que o HttpClient seja descartado corretamente
        //        handler.Dispose();     // Descartar o handler também é importante
        //    }
        //}

        public async Task<ServiceResult<ResponseBodyItau>> NewCobPixQRCODE(string token, string txid, RequestDataItau requestData)
        {
            ServiceResult<ResponseBodyItau> _serviceResult = new ServiceResult<ResponseBodyItau>();
            string url = _configuration.GetSection("UrlsPixItau:qrcode_pix").Value + txid;

            try
            {
                // Adicionar o token ao header de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Convertendo o objeto de requestData para JSON
                var jsonString = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                Console.WriteLine($"Request URL: {url}");
                Console.WriteLine($"Request Body: {jsonString}");

                var response = await _httpClient.PutAsync(url, content);

                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    Console.WriteLine($"Response Body: {responseBody}");
                    _serviceResult.Error = responseBody;
                    return _serviceResult;
                }


                ResponseBodyItau responseData = JsonSerializer.Deserialize<ResponseBodyItau>(responseBody);

                _serviceResult.Result = responseData;

                return _serviceResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }
        }

        public async Task<ServiceResult<List<ResponseListPixItau>>> ListaPix(string dataInicio, string dataFim, string token)
        {
            ServiceResult<List<ResponseListPixItau>> _serviceResult = new ServiceResult<List<ResponseListPixItau>>();
            string baseUrl = _configuration.GetSection("UrlsPixItau:lista_pix").Value;

            try
            {
                // Adicionar o token ao header de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Montar a URL com os parâmetros de query
                string url = $"{baseUrl}?inicio={dataInicio}&fim={dataFim}";

                Console.WriteLine($"Request URL: {url}");

                var response = await _httpClient.GetAsync(url);

                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    Console.WriteLine($"Response Body: {responseBody}");
                    _serviceResult.Error = responseBody;
                    return _serviceResult;
                }

                List<ResponseListPixItau> responseData = JsonSerializer.Deserialize<List<ResponseListPixItau>>(responseBody);

                _serviceResult.Result = responseData;

                return _serviceResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }
        }

        public async Task<ServiceResult<ResponseBodyItau>> ConsultarPix(string txId, string token)
        {
            ServiceResult<ResponseBodyItau> _serviceResult = new ServiceResult<ResponseBodyItau>();
            string _txtId = MontarTxtID(txId);
            string url = _configuration.GetSection("UrlsPixItau:consulta_pix").Value + "/" + _txtId;

            try
            {
                // Adicionar o token ao header de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Montar a URL com os parâmetros de query
                var response = await _httpClient.GetAsync(url);

                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    Console.WriteLine($"Response Body: {responseBody}");
                    _serviceResult.Error = responseBody;
                    return _serviceResult;
                }

                ResponseBodyItau responseData = JsonSerializer.Deserialize<ResponseBodyItau>(responseBody);

                _serviceResult.Result = responseData;

                return _serviceResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                _serviceResult.Error = ex.Message;
                return _serviceResult;
            }
        }

        private string MontarTxtID(string txId)
        {
            const string baseString = "banddeicpix";
            int neededLength = 35 - baseString.Length;
            string formattedNumber = txId.PadLeft(neededLength, '0');
            return baseString + formattedNumber;
        }

    }
}
