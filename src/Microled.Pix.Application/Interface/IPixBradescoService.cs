using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;

namespace Microled.Pix.Application.Interface
{
    public interface IPixBradescoService
    {
        Task<ServiceResult<PagamentoResponse>> CreateNewQrCodePix(PagamentoRequest request);
        Task<ServiceResult<PagamentoResponse>> ConsultarQrCodePix(string txId);
    }
}
