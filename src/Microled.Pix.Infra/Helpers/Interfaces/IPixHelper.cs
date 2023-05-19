using Microled.Pix.Domain.Response;
using Microled.Pix.Domain.ViewModel;

namespace Microled.Pix.Infra.Helpers.Interfaces
{
    public interface IPixHelper
    {
        Task<string> getAuthenticationToken(BankCredentials credentials);
    }
}
