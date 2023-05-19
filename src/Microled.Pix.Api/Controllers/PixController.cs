using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Interfaces;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace Microled.Pix.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PixController : ControllerBase
{
    private readonly ILogger<PixController> _logger;
    private readonly IPixQrCodeService _qrCodeService;

    public PixController(ILogger<PixController> logger, IPixQrCodeService pixQrCodeService)
    {
        _logger = logger;
        _qrCodeService = pixQrCodeService;
    }

    [HttpPost]
    public async Task<PagamentoResponse> CriarPagamentoQrCodeComVencimento([FromBody] PagamentoRequest request)
    {
        return await _qrCodeService.CreateNewQrCodePix(request);
    }
}
