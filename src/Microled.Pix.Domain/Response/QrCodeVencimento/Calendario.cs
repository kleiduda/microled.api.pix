namespace Microled.Pix.Domain.Response.QrCodeVencimento
{
    public class Calendario
    {
        public DateTime criacao { get; set; }
        public string? dataDeVencimento { get; set; }
        public int validadeAposVencimento { get; set; }
    }
}
