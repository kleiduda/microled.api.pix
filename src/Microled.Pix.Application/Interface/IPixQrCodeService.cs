using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;

namespace Microled.Pix.Application.Interface
{
    public interface IPixQrCodeService
    {
        Task<PagamentoResponse> CreateNewQrCodePix(PagamentoRequest request);
    }
}
