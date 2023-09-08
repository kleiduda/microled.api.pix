using Microled.Pix.Domain.Enum;

namespace Microled.Pix.Domain.Response
{
    public class PagamentoResponse
    {
        public int IdEmpresa { get; set; }
        public int Pagamento { get; set; } //id do pagamento
        public string Empresa { get; set; } // Razao social empresa
        public int Processo { get; set; } //roger precisa validar do que se trata
        public string NumeroTitulo { get; set; }
        public EStatusPagamento StatusPagamento { get; set; }
        public string QRCode_Imagem_base64 { get; set; }
        public string Pix_Link { get; set; }
        public string QRCode_Texto_EMV { get; set; }
        public double ValorRet { get; set; }
        public List<string> Emails_Aviso_Pagamento { get; set; }

    }
}
