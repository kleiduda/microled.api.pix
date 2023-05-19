using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.ViewModel;
using Microled.Pix.Infra.Helpers.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace Microled.Pix.Application
{
    public class PixQrCodeService : IPixQrCodeService
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
            PagamentoResponse response = new PagamentoResponse();
            BankCredentials credentials = new BankCredentials();
            credentials.ClientId = _configuration.GetSection("CredenciaisItau:client_id").Value;
            credentials.ClientSecret = _configuration.GetSection("CredenciaisItau:client_secret").Value;
            //buscar token de autenticacao
            string accesToken = await _helper.getAuthenticationToken(credentials);
            //fazer requisicao ao endpoint do pix itau

            //resgatar retorno da api do pix

            //guardar o retorno no database, ou deixar por conta do sistema da microled, apenas retornar o json 

            return response;
        }
    }
}
