using Microled.Pix.Application.Interface;
using Microled.Pix.Domain.Request;
using Microled.Pix.Domain.Request.Itau;
using Microled.Pix.Domain.Request.Itau.Cancelamento;
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

    [HttpGet]
    [Route("itau/token")]
    public async Task<ServiceResult<TokenResponse>> GetToken()
    {
        return await _qrCodeService.GetToken();
    }

    [HttpPost]
    [Route("itau/pix")]
    public async Task<ServiceResult<PagamentoResponse>> CriarPagamentoQrCodeComVencimento([FromBody] PagamentoRequest request)
    {
        return await _qrCodeService.CreateNewQrCodePix(request);
    }

    [HttpPost]
    [Route("itau/pix/consulta")]
    public async Task<ServiceResult<PagamentoResponse>> GetPixConsulta([FromBody] ConsultaRequest request)
    {
        return await _qrCodeService.ConsultaPix(request.IdPagamento.ToString(), request.Token);
    }

    [HttpPost]
    [Route("itau/pix/cancelamento")]
    public async Task<ServiceResult<PagamentoResponse>> CancelarPagamentoPix([FromBody] CancelamentoRequest request)
    {
        //ServiceResult<PagamentoResponse> _serviceResult = new ServiceResult<PagamentoResponse>();
        //Metodo que cancela o PIX Itau
        return await _qrCodeService.CancelamentoPix(request);
        
    }

    [HttpPost]
    [Route("itau/pix/baixa")]
    public async Task<ServiceResult<string>> BaixaTituloPix([FromBody] BaixaRequest request)
    {
        return await _qrCodeService.BaixaTituloPix(request.IdPagamento.ToString());
    }
}
