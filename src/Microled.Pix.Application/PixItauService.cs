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
using Microled.Pix.Domain.Request.Itau;
using Microled.Pix.Domain.Request.Itau.Cancelamento;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net;

namespace Microled.Pix.Application
{
    public class PixItauService : IPixItauService
    {
        private readonly IPixItauHelper _helper;
        private readonly IConfiguration _configuration;
        public PixItauService(IPixItauHelper pixHelper, IConfiguration configuration)
        {
            _helper = pixHelper;
            _configuration = configuration;
        }

        public async Task<ServiceResult<PagamentoResponse>> CreateNewQrCodePix(PagamentoRequest request)
        {
            string chavePix = _configuration.GetSection("CredenciaisItau:chave_pix").Value;
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            BankCredentials credentials = new BankCredentials();
            //credentials.ClientId = _configuration.GetSection("CredenciaisItau:client_id").Value;
            //credentials.ClientSecret = _configuration.GetSection("CredenciaisItau:client_secret").Value;

            try
            {
                if (!string.IsNullOrEmpty(request.Token))
                {
                    //requisicao eo endpoint de criar QRCODE
                    RequestDataItau dataItau = RequestDataItau.Create(Domain.Request.Itau.Calendario.Create(request.Data_Hora_Expiracao_Pagamento, 1),
                                                                      Domain.Request.Itau.Devedor.Create(request.Devedor_CPF, request.Devedor_CNPJ, request.Devedor_Nome),
                                                                      Domain.Request.Itau.Valor.Create(request.Valor),
                                                                      chavePix);

                    var response = await _helper.NewCobPixQRCODE(request.Token, request.GerarTxID(), dataItau);
                    //_serviceResult.Mensagens = new List<string>() { "Token não validado!" + accesToken };
                    _serviceResult.Result = PagamentoResponse.CriarPagamento(1, 
                                                                             response.Result.txid, 
                                                                             response.Result.recebedor.nome,
                                                                             1,
                                                                             request.Numero_Titulo,
                                                                             response.Result.status,
                                                                             GenerateQRCodeBase64(response.Result.pixCopiaECola),
                                                                             response.Result.loc.location,
                                                                             response.Result.pixCopiaECola,
                                                                             Convert.ToDecimal(response.Result.valor.original),
                                                                             request.Emails_Aviso_Pagamento
                                                                             );
                    //{
                    //    IdEmpresa = 1,
                    //    Pagamento = Convert.ToInt32(request.Numero_Titulo), //verificar de onde vem esse valor
                    //    Empresa = "DEICMAR BANDEIRANTES",//
                    //    Processo = 1, //vericiar que valor seria esse
                    //    NumeroTitulo = request.Numero_Titulo,
                    //    StatusPagamento = "ATIVA",
                    //    QRCode_Imagem_base64 = GenerateQRCodeBase64("http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA"),
                    //    Pix_Link = "http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                    //    QRCode_Texto_EMV = "http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                    //    ValorRet = request.Valor,
                    //    Emails_Aviso_Pagamento = new List<string>() { request.Emails_Aviso_Pagamento.FirstOrDefault() }
                    //};
                }
                else
                {
                    _serviceResult.Mensagens = new List<string>() { "Token Invalido" };

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
        public async Task<ServiceResult<TokenResponse>> GetToken()
        {
            ServiceResult<TokenResponse> serviceResult = new ServiceResult<TokenResponse>();

            BankCredentials credentials = new BankCredentials();
            credentials.ClientId = _configuration.GetSection("CredenciaisItau:client_id").Value;
            credentials.ClientSecret = _configuration.GetSection("CredenciaisItau:client_secret").Value;

            string getAccessToken = await _helper.GetAuthenticationToken(credentials);
            AccessToken accessToken = JsonSerializer.Deserialize<AccessToken>(getAccessToken);

            serviceResult.Result = TokenResponse.CreateNewToken(accessToken.access_token, accessToken.expires_in);
            return serviceResult;
        }

        public async Task<ServiceResult<PagamentoResponse>> CancelamentoPix(CancelamentoRequest request)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            if (request.IdPagamento == 1)
            {
                _serviceResult.Mensagens = new List<string>() { "Cobrança PIX cancelada com sucesso." };
                _serviceResult.Result = new PagamentoResponse()
                {
                    IdEmpresa = 1,
                    Pagamento ="1", //verificar de onde vem esse valor
                    Empresa = "DEICMAR BANDEIRANTES",
                    Processo = 1, //vericiar que valor seria esse
                    NumeroTitulo = request.NumeroTitulo,
                    StatusPagamento = "CANCELADA",
                    QRCode_Imagem_base64 = GenerateQRCodeBase64("http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA"),
                    Pix_Link = "http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                    QRCode_Texto_EMV = "http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                    ValorRet = 5,
                    Emails_Aviso_Pagamento = new List<string>() { "roger@microled.com.br" }
                };
            }
            else if (request.IdPagamento == 2)
            {
                _serviceResult.Error = "Erro no cancelamento do Pix";
            }
            else
            {
                _serviceResult.Mensagens = new List<string>() { "Pix nao encontrado!" };
            }

            return _serviceResult;
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

        public async Task<ServiceResult<PagamentoResponse>> ConsultaPix(string txId, string token)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            _serviceResult.Result = new PagamentoResponse()
            {
                IdEmpresa = 1,
                Pagamento = txId, //verificar de onde vem esse valor
                Empresa = "DEICMAR BANDEIRANTES",
                Processo = 1, //vericiar que valor seria esse
                NumeroTitulo = txId,
                StatusPagamento = "CANCELADA",
                QRCode_Imagem_base64 = GenerateQRCodeBase64("http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA"),
                Pix_Link = "http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                QRCode_Texto_EMV = "http://h.itau.com.br/qr/v2/1eb17258-88ec-4078-80e4-03ae10fb594f5204000053039865406100.005802BR5924CONTA",
                ValorRet = 5,
                Emails_Aviso_Pagamento = new List<string>() { "roger@microled.com.br" }
            };
            return _serviceResult;
        }

        public async Task<ServiceResult<string>> BaixaTituloPix(string txId)
        {
            string token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJVc3VhcmlvVGVzdGUiLCJVc3VhcmlvVGVzdGUiXSwianRpIjoiOGJiMDgwNWZhMzk0NGFmNGE4ZWViYzAyODk0Yjc0MTUiLCJuYmYiOjE2OTA0NjE5MTQsImV4cCI6MTY5MDQ5MDcxNCwiaWF0IjoxNjkwNDYxOTE0fQ.NcJ7urfh5vQwWWip8rf07JhGO_HZVNTp1LCXgJ8rk7v5YOWVYiL43DilYd26tlzyQO9iNfILB-dTKJk2DQU4RSDEkwoiC8hY4oBPpUl9RYc-acSBWpUjBlMOKyyIOQnI5oXrKSOA2JABa6uwMx1338gYSzUcq9jp6Qe9s9DzhNREmpJWOOXwB5crHNlG2LtG91yRC5M3xGU0fIRNVoArBGMnu7rJMNhKlDpARm9SpCwASU2TLVlEoRvS9ZjEsdI5RWIVaTZe5xWmBjSTn8NijYsLPnXtWGHRVpMFKrGU74xfzTo2KJlUXaUkiUEDtbDTV-zUC0zzkMEw-z-9Iu3wGg";
            ServiceResult<string> _serviceResult = new ServiceResult<string>();
            var ret = await _helper.BaixaTitulo(txId, token);

            return ret;
        }
    }
}
