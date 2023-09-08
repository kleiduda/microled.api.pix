using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace Microled.Pix.Api.Controllers;

[ApiController]
[Route("microled")]
public class ItauController : ControllerBase
{
    private readonly ILogger<ItauController> _logger;
    private readonly IPixItauService _qrCodeService;

    public ItauController(ILogger<ItauController> logger, IPixItauService pixQrCodeService)
    {
        _logger = logger;
        _qrCodeService = pixQrCodeService;
    }

    [HttpPost]
    [Route("itau/pix")]
    public async Task<ServiceResult<PagamentoResponse>> CriarPagamentoQrCodeComVencimento([FromBody] PagamentoRequest request)
    {
        return await _qrCodeService.CreateNewQrCodePix(request);
    }
}
