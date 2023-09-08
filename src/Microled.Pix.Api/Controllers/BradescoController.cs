using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace Microled.Pix.Api.Controllers;

[ApiController]
[Route("microled")]
public class BradescoController : ControllerBase
{
    private readonly ILogger<BradescoController> _logger;
    private readonly IPixBradescoService _qrCodeService;

    public BradescoController(ILogger<BradescoController> logger, IPixBradescoService pixQrCodeService)
    {
        _logger = logger;
        _qrCodeService = pixQrCodeService;
    }

    [HttpPost]
    [Route("bradesco/pix")]
    [ProducesResponseType(typeof(IServiceResult<int>), 200)]
    public async Task<ServiceResult<PagamentoResponse>> CriarPagamentoQrCodeComVencimento([FromBody] PagamentoRequest request)
    {
        ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse> ();
        if (!request.IsValid)
        {
            _serviceResult.Mensagens = request.Notifications.Select(x => x.Message).ToList();

            return _serviceResult;
        }

        var result = await _qrCodeService.CreateNewQrCodePix(request);
        return result;
    }
}
