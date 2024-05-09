using Microled.Pix.Domain.Enum;

namespace Microled.Pix.Domain.Response
{
    public class PagamentoResponse
    {
        public int IdEmpresa { get; set; }
        public string Pagamento { get; set; } //id do pagamento
        public string Empresa { get; set; } // Razao social empresa
        public int Processo { get; set; } //roger precisa validar do que se trata
        public string NumeroTitulo { get; set; }
        public string StatusPagamento { get; set; }
        public string QRCode_Imagem_base64 { get; set; }
        public string Pix_Link { get; set; }
        public string QRCode_Texto_EMV { get; set; }
        public decimal ValorRet { get; set; }
        public List<string> Emails_Aviso_Pagamento { get; set; }

        public static PagamentoResponse CriarPagamento(int idEmpresa, string pagamento, string empresa, int processo, string numeroTitulo, string statusPagamento, string qrCode,
            string pixLink, string qrcodeTexto, decimal valorRet, List<string> emails)
        {
            var pagto = new PagamentoResponse();
            pagto.IdEmpresa = idEmpresa;
            pagto.Pagamento = pagamento;
            pagto.Empresa = empresa;
            pagto.Processo = processo;
            pagto.NumeroTitulo = numeroTitulo;
            pagto.StatusPagamento = statusPagamento;
            pagto.QRCode_Imagem_base64 = qrCode;
            pagto.Pix_Link = pixLink;
            pagto.QRCode_Texto_EMV = qrcodeTexto;
            pagto.ValorRet = valorRet;
            pagto.Emails_Aviso_Pagamento = emails;

            return pagto;
        }
    }
}
