namespace Microled.Pix.Domain.Request
{

    public class PagamentoRequest
    {
        public Calendario Calendario { get; set; }
        public Devedor Devedor { get; set; }
        public string Valor { get; set; }
        public string Chave { get; set; }
    }

    public class Calendario
    {
        public DateTime DataDeVencimento { get; set; }
    }

    public class Devedor
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
    }
}
