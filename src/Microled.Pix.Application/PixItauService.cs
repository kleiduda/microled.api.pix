﻿using Microled.Pix.Application.Interface;
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
using System.Security.Cryptography.X509Certificates;

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
                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        _serviceResult.Error = response.Error;
                    }
                    else
                    {
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
                    }

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
        //public async Task<ServiceResult<TokenResponse>> GetToken()
        //{
        //    ServiceResult<TokenResponse> serviceResult = new ServiceResult<TokenResponse>();

        //    try
        //    {
        //        BankCredentials credentials = new BankCredentials();
        //        credentials.ClientId = _configuration.GetSection("CredenciaisItau:client_id").Value;
        //        credentials.ClientSecret = _configuration.GetSection("CredenciaisItau:client_secret").Value;

        //        string getAccessToken = await _helper.GetAuthenticationToken(credentials);
        //        AccessToken accessToken = JsonSerializer.Deserialize<AccessToken>(getAccessToken);

        //        serviceResult.Result = TokenResponse.CreateNewToken(accessToken.access_token, accessToken.expires_in);
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResult.Error = ex.Message;
        //        throw ex;
        //    }

        //    return serviceResult;
        //}

        public async Task<ServiceResult<TokenResponse>> GetToken()
        {
            ServiceResult<TokenResponse> serviceResult = new ServiceResult<TokenResponse>();

            try
            {
                BankCredentials credentials = new BankCredentials
                {
                    ClientId = _configuration.GetSection("CredenciaisItau:client_id").Value,
                    ClientSecret = _configuration.GetSection("CredenciaisItau:client_secret").Value
                };

                string getAccessToken = await GetAuthenticationToken(credentials);
                AccessToken accessToken = JsonSerializer.Deserialize<AccessToken>(getAccessToken);

                serviceResult.Result = TokenResponse.CreateNewToken(accessToken.access_token, accessToken.expires_in);
            }
            catch (Exception ex)
            {
                serviceResult.Error = ex.Message;
                throw;
            }

            return serviceResult;
        }

        public async Task<string> GetAuthenticationToken(BankCredentials credentials)
        {
            string url = _configuration.GetSection("UrlsPixItau:authentication").Value;

            try
            {
                // Carregar o certificado PFX
                string pfxFilePath = _configuration.GetSection("CredenciaisItau:certificado").Value;
                string pfxPassword = _configuration.GetSection("CredenciaisItau:password").Value;
                var certificate = new X509Certificate2(pfxFilePath, pfxPassword);

                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);

                using var client = new HttpClient(handler);

                // Configurar os cabeçalhos da requisição
                client.DefaultRequestHeaders.Add("x-itau-flowID", "1");
                client.DefaultRequestHeaders.Add("x-itau-correlationID", "2");

                // Construir o corpo da requisição com client_id e client_secret
                var requestData = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("client_secret", credentials.ClientSecret)
                });

                var response = await client.PostAsync(url, requestData);

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
                throw new Exception($"Failed to get authentication token: {ex.Message}", ex);
            }
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
                    Pagamento = "1", //verificar de onde vem esse valor
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
            BankCredentials credentials = new BankCredentials();

            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var response = await _helper.ConsultarPix(txId, token);
                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        _serviceResult.Error = response.Error;
                    }
                    else
                    {
                        
                    }

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

        public async Task<ServiceResult<string>> BaixaTituloPix(string txId)
        {
            string token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJVc3VhcmlvVGVzdGUiLCJVc3VhcmlvVGVzdGUiXSwianRpIjoiOGJiMDgwNWZhMzk0NGFmNGE4ZWViYzAyODk0Yjc0MTUiLCJuYmYiOjE2OTA0NjE5MTQsImV4cCI6MTY5MDQ5MDcxNCwiaWF0IjoxNjkwNDYxOTE0fQ.NcJ7urfh5vQwWWip8rf07JhGO_HZVNTp1LCXgJ8rk7v5YOWVYiL43DilYd26tlzyQO9iNfILB-dTKJk2DQU4RSDEkwoiC8hY4oBPpUl9RYc-acSBWpUjBlMOKyyIOQnI5oXrKSOA2JABa6uwMx1338gYSzUcq9jp6Qe9s9DzhNREmpJWOOXwB5crHNlG2LtG91yRC5M3xGU0fIRNVoArBGMnu7rJMNhKlDpARm9SpCwASU2TLVlEoRvS9ZjEsdI5RWIVaTZe5xWmBjSTn8NijYsLPnXtWGHRVpMFKrGU74xfzTo2KJlUXaUkiUEDtbDTV-zUC0zzkMEw-z-9Iu3wGg";
            ServiceResult<string> _serviceResult = new ServiceResult<string>();
            var ret = await _helper.BaixaTitulo(txId, token);

            return ret;
        }

        public async Task<ServiceResult<PagamentoResponse>> ListaPix(string dataInicio, string dataFim, string token)
        {
            ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
            BankCredentials credentials = new BankCredentials();

            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var response = await _helper.ListaPix(dataInicio, dataFim, token);
                    if (!string.IsNullOrEmpty(response.Error))
                    {
                        _serviceResult.Error = response.Error;
                    }
                    else
                    {
                        //_serviceResult.Result = PagamentoResponse.CriarPagamento(1,
                        //                                                    response.Result.txid,
                        //                                                    response.Result.recebedor.nome,
                        //                                                    1,
                        //                                                    request.Numero_Titulo,
                        //                                                    response.Result.status,
                        //                                                    GenerateQRCodeBase64(response.Result.pixCopiaECola),
                        //                                                    response.Result.loc.location,
                        //                                                    response.Result.pixCopiaECola,
                        //                                                    Convert.ToDecimal(response.Result.valor.original),
                        //                                                    request.Emails_Aviso_Pagamento
                        //                                                    );
                    }

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
    }
}
