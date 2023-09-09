using Microled.Pix.Domain.Request.Bradesco;
using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.ViewModel;

namespace Microled.Pix.Infra.Helpers.Interfaces
{
    public interface IPixBradescoHelper
    {
        Task<string> GetAuthenticationToken(BankCredentials credentials);
        Task<ServiceResult<PagamentoResponse>> UpdateCobvEmvData(string token, RequestDataBradesco requestData);
        Task<ServiceResult<PagamentoResponse>> ConsultarQrCodePix(string token, string txId);

    }
}
