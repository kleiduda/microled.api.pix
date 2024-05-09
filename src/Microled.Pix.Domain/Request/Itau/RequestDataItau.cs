namespace Microled.Pix.Domain.Request.Itau
{
    public  class RequestDataItau
    {
        public Calendario Calendario { get; set; }
        public Devedor Devedor { get; set; }
        public Valor Valor { get; set; }
        public string Chave { get; set; }

        public static RequestDataItau Create(Calendario calendario, Devedor devedor, Valor valor, string chave)
        {
            RequestDataItau request = new RequestDataItau();
            request.Calendario = calendario;
            request.Devedor = devedor;
            request.Valor = valor;
            request.Chave = chave;

            return request;
        }

    }
    public class Calendario
    {
        public string DataDeVencimento { get; set; }
        public int ValidadeAposVencimento { get; set; }
        public static Calendario Create(string dataVencimento, int validadeAposVencimento)
        {
            var calendario = new Calendario();
            calendario.DataDeVencimento = dataVencimento;
            calendario.ValidadeAposVencimento= validadeAposVencimento;

            return calendario;
        }
    }

    public class Devedor
    {
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string Nome { get; set; }

        public static Devedor Create(string cpf, string cnpj, string nome)
        {
            var devedor = new Devedor();
            devedor.Cpf = cpf;
            devedor.Cnpj = cnpj;
            devedor.Nome = nome;

            return devedor;
        }
    }

    public class Valor
    {
        public decimal Original { get; set; }

        public static Valor Create(decimal original)
        {
            Valor valor = new Valor();
            valor.Original = original;

            return valor;
        }
    }
}
