using Microled.Pix.Domain.Request.Bradesco;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.ViewModel;

namespace Microled.Pix.Infra.Helpers.Interfaces
{
    public interface IPixItauHelper
    {
        Task<string> GetAuthenticationToken(BankCredentials credentials);
        Task<ServiceResult<PagamentoResponse>> UpdateCobvEmvData(string token, string txid, RequestDataBradesco requestData);
    }
}
