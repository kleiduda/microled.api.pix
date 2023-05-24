using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response.QrCodeVencimento;

namespace Microled.Pix.Application.Interface
{
    public interface IPixQrCodeService
    {
        Task<PagamentoResponse> CreateNewQrCodePix(PagamentoRequest request);
    }
}
