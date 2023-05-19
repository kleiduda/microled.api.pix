using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;

namespace Microled.Pix.Domain.Interfaces
{
    public interface IPixQrCodeDomain
    {
        Task<PagamentoResponse> CriarPagamentoQrCode(PagamentoRequest request);
    }
}
