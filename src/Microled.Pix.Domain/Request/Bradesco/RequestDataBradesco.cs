namespace Microled.Pix.Domain.Request.Bradesco
{
    public  class RequestDataBradesco
    {
        public Calendario Calendario { get; set; }
        public Devedor Devedor { get; set; }
        public Valor Valor { get; set; }
        public string Chave { get; set; }
        public string SolicitacaoPagador { get; set; }

    }
    public class Calendario
    {
        public string DataDeVencimento { get; set; }
        public int ValidadeAposVencimento { get; set; } 
    }

    public class Devedor
    {
        public string? Logradouro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Cep { get; set; }
        public string? Cpf { get; set; }
        public string? Nome { get; set; }
    }

    public class Multa
    {
        public string? Modalidade { get; set; }
        public string? ValorPerc { get; set; }
    }

    public class Juros
    {
        public string? Modalidade { get; set; }
        public string? ValorPerc { get; set; }
    }

    public class DescontoDataFixa
    {
        public string? Data { get; set; }
        public string? ValorPerc { get; set; }
    }

    public class Desconto
    {
        public string? Modalidade { get; set; }
        public List<DescontoDataFixa>? DescontoDataFixa { get; set; }
    }

    public class Valor
    {
        public decimal Original { get; set; }
        public Multa? Multa { get; set; }
        public Juros? Juros { get; set; }
        public Desconto? Desconto { get; set; }
    }
}
