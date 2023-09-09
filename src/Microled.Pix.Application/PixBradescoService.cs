using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microled.Pix.Domain.Request.Bradesco;
using System.Buffers.Text;
using System.Drawing;
using System.Net.NetworkInformation;
using ZXing;
using Microled.Pix.Domain.Enum;
using System.Text.Json;

namespace Microled.Pix.Application
{
    public class PixBradescoService : IPixBradescoService
    {
        private readonly IPixBradescoHelper _helper;
        private readonly IConfiguration _configuration;
        public PixBradescoService(IPixBradescoHelper pixHelper, IConfiguration configuration)
        {
            _helper = pixHelper;
            _configuration = configuration;
        }

        public async Task<ServiceResult<PagamentoResponse>> CreateNewQrCodePix(PagamentoRequest request)
        {
            string chavePix = _configuration.GetSection("CredenciaisBradesco:chave-pix").Value ?? ""; //CHAVE PIX DE HOMOLOG
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            BankCredentials credentials = new BankCredentials();
            credentials.ClientId = _configuration.GetSection("CredenciaisBradesco:client_id").Value ?? "";
            credentials.ClientSecret = _configuration.GetSection("CredenciaisBradesco:client_secret").Value ?? "";

            try
            {
                string tokenJson = await _helper.GetAuthenticationToken(credentials);
                TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenJson);

                string accessToken = tokenResponse.access_token;

                //fazer requisicao ao endpoint do pix vencimento bradesco
                if (accessToken.Contains("Error"))
                {
                    _serviceResult.Mensagens = new List<string>() { "Token não validado!" + accessToken };
                    _serviceResult.Result = new PagamentoResponse()
                    {
                        IdEmpresa = 1,
                        Pagamento = "", //verificar de onde vem esse valor
                        Empresa = "DEICMAR BANDEIRANTES",
                        Processo = 1, //vericiar que valor seria esse
                        NumeroTitulo = request.Numero_Titulo,
                        StatusPagamento = "ATIVA",
                        QRCode_Imagem_base64 = GenerateQRCodeBase64("http://h.bradesco.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA"),
                        Pix_Link = "http://h.bradesco.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                        QRCode_Texto_EMV = "http://h.bradesco.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                        ValorRet = request.Valor,
                        Emails_Aviso_Pagamento = new List<string>() { request.Emails_Aviso_Pagamento.FirstOrDefault() }
                    };
                }
                else
                {
                    var requestData = new RequestDataBradesco()
                    {
                        Calendario = new Calendario() { DataDeVencimento = request.Data_Hora_Expiracao_Pagamento },
                        Devedor = new Devedor() { Nome = request.Devedor_Nome, Cpf = request.Devedor_CPF },
                        Valor = new Valor() { Original = request.Valor },
                        Chave = chavePix,
                        SolicitacaoPagador = request.Solicitacao_Pagador
                    };

                    _serviceResult = await _helper.UpdateCobvEmvData(accessToken, requestData);

                }

                return _serviceResult;
            }
            catch (Exception ex)
            {
                _serviceResult.Error = ex.Message;
                _serviceResult.Mensagens = new List<string>() { "Erro ao fazer a requisicao para API Bradesco:" + ex.Message };
                return _serviceResult;
            }

        }

        public async Task<ServiceResult<PagamentoResponse>> ConsultarQrCodePix(string txId)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            BankCredentials credentials = new BankCredentials();
            credentials.ClientId = _configuration.GetSection("CredenciaisBradesco:client_id").Value ?? "";
            credentials.ClientSecret = _configuration.GetSection("CredenciaisBradesco:client_secret").Value ?? "";

            try
            {
                string tokenJson = await _helper.GetAuthenticationToken(credentials);
                TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenJson);

                string accessToken = tokenResponse.access_token;
                _serviceResult = await _helper.ConsultarQrCodePix(accessToken, txId);

                return _serviceResult;
            }
            catch (Exception ex)
            {
                _serviceResult.Error = ex.Message;
                _serviceResult.Mensagens = new List<string>() { "Erro na consulta API Bradesco:" + ex.Message };
                return _serviceResult;
            }

        }

        public string GenerateQRCodeBase64(string data)
        {
            var qrCodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 300,
                    Width = 300
                }
            };

            var pixelData = qrCodeWriter.Write(data);
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using var ms = new MemoryStream();
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
               System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                    pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var base64String = Convert.ToBase64String(ms.ToArray());

            return base64String;
        }


    }
}
