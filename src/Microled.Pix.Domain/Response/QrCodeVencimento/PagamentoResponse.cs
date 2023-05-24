namespace Microled.Pix.Domain.Response.QrCodeVencimento
{
    public class PagamentoResponse
    {
        //public Calendario calendario { get; set; }
        public string txid { get; set; }
        //public int revisao { get; set; }
        //public Loc loc { get; set; }
        public string location { get; set; }
        public string status { get; set; }
        //public Devedor devedor { get; set; }
        //public Recebedor recebedor { get; set; }
        //public Valor valor { get; set; }
        public string valor { get; set; } 
        public string chave { get; set; }
        //public string solicitacaoPagador { get; set; }
    }
}
