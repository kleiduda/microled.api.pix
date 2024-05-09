namespace Microled.Pix.Domain.Request.Itau.Cancelamento
{
    public class CancelamentoRequest
    {
        public int IdPagamento { get; set; }
        public string NumeroTitulo { get; set; }
        public string token { get; set; }
    }
}
