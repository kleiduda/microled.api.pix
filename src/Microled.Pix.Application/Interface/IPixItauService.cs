using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Request.Itau.Cancelamento;
using Microled.Pix.Domain.Response;

namespace Microled.Pix.Application.Interface
{
    public interface IPixItauService
    {
        Task<ServiceResult<TokenResponse>> GetToken();
        Task<ServiceResult<PagamentoResponse>> CreateNewQrCodePix(PagamentoRequest request);
        Task<ServiceResult<PagamentoResponse>> CancelamentoPix(CancelamentoRequest request);
        Task<ServiceResult<PagamentoResponse>> ConsultaPix(string txId, string token);
        Task<ServiceResult<string>> BaixaTituloPix(string txId);
    }
}
