using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response.QrCodeVencimento;
using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microled.Pix.Infra.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Text;
using System.Drawing.Imaging;

namespace Microled.Pix.Application
{
    public class PixQrCodeService : BaseHttpClient, IPixQrCodeService
    {
        private readonly IPixHelper _helper;
        private readonly IConfiguration _configuration;
        public PixQrCodeService(IPixHelper pixHelper, IConfiguration configuration)
        {
            _helper = pixHelper;
            _configuration = configuration;
        }

        public async Task<PagamentoResponse> CreateNewQrCodePix(PagamentoRequest request)
        {
            #region FAKE OBJECT
            //TXID fake
            string txId = "MICROLED123412340000000000";
            PagamentoResponse _responseMock = null;
            _responseMock = new PagamentoResponse()
            {
                txid = txId,
                valor = request.Valor,
                location = "https://pixmicroled/teste/id",
                chave = request.Chave
            };
            #endregion
            string accessToken = string.Empty;
            //if (request.Bradesco)
            //{

            //}
            //else
            //{

            //}

            PagamentoResponse response = new PagamentoResponse();
            BankCredentials credentials = new BankCredentials();
            credentials.ClientId = _configuration.GetSection("CredenciaisItau:client_id").Value;
            credentials.ClientSecret = _configuration.GetSection("CredenciaisItau:client_secret").Value;
            //buscar token de autenticacao
            string accessTokenJson = await _helper.getAuthenticationToken(credentials);
            JsonDocument jsonDocument = JsonDocument.Parse(accessTokenJson);
            if (jsonDocument.RootElement.GetProperty("access_token").GetString() != null)
            {
                accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString();
            }

            string urlRequest = _configuration.GetSection("UrlsPix:sandbox_url").Value + "pix_recebimentos/cobv/" + txId;
            //fazer requisicao ao endpoint do pix itau
            var _response = await SendAsync(HttpMethod.Put, urlRequest, JsonSerializer.Serialize(request), accessToken);
            //gerar qrcode
            var qrCode = GerarQrCodeBase64(_responseMock);

            //guardar o retorno no database, ou deixar por conta do sistema da microled, apenas retornar o json 

            return _responseMock;
        }

        public string GerarQrCodeBase64(PagamentoResponse pagamento)
        {
            // Criar o QR Code a partir do JSON do objeto de Pix
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(pagamento.location, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);

            // Converter o QR Code em uma imagem
            var qrCodeImage = qrCode.GetGraphic(20);

            // Converter a imagem em base64
            using (var memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, ImageFormat.Png);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public void WebHookPagamentosPix()
        {
            _helper.VerificarPagamentosPix();
        }
    }
}
