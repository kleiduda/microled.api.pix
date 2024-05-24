namespace Microled.Pix.Domain.Response.itau.cancelamento
{
    public class CancelamentoResponse
    {
        public Loc loc { get; set; }
        public Devedor devedor { get; set; }
        public Valor valor { get; set; }
        public string solicitacaoPagador { get; set; }

        public static CancelamentoResponse New(Loc loc, Devedor devedor, Valor valor, string solicitacaoPagador)
        {
            var cancelamento = new CancelamentoResponse();
            cancelamento.devedor = devedor;
            cancelamento.loc = loc;
            cancelamento.valor = valor;
            cancelamento.solicitacaoPagador = solicitacaoPagador;
            return cancelamento;

        }
    }

    public class Devedor
    {
        public string Logradouro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
        public string Cnpj { get; set; }
        public string Nome { get; set; }

        public static Devedor New(string logradouro, string cidade, string uf, string cep, string cnpj, string nome)
        {
            var devedor = new Devedor();
            devedor.Logradouro =logradouro;
            devedor.Cidade = cidade;
            devedor.Uf = uf;
            devedor.Cep = cep;
            devedor.Cnpj = cnpj;
            devedor.Nome = nome;

            return devedor;
        }
    }   

    public class Loc
    {
        public long id { get; set; }
        public static Loc New(long id){
            var loc = new Loc();
            loc.id = id;

            return loc;
        }
    }

    public class Valor
    {
        public string original { get; set; }
        public static Valor New(string original){
            var valor = new Valor();

            valor.original = original;
            return valor;
        }
    }
}