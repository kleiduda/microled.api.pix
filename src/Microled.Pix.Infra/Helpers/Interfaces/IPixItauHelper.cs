using Microled.Pix.Domain.Request.Bradesco;
using Microled.Pix.Domain.Request.Itau;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.Response.itau;
using Microled.Pix.Domain.ViewModel;

namespace Microled.Pix.Infra.Helpers.Interfaces
{
    public interface IPixItauHelper
    {
        Task<string> GetAuthenticationToken(BankCredentials credentials);
        Task<ServiceResult<ResponseBodyItau>> NewCobPixQRCODE(string token, string txid, RequestDataItau requestData);
        Task<ServiceResult<string>> BaixaTitulo(string txId, string token);
    }
}
