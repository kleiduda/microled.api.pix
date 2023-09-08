using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;

namespace Microled.Pix.Application.Interface
{
    public interface IPixItauService
    {
        Task<ServiceResult<PagamentoResponse>> CreateNewQrCodePix(PagamentoRequest request);
    }
}
