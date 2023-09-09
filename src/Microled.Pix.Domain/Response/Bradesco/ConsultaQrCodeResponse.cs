namespace Microled.Pix.Domain.Response.Bradesco
{

    public class ConsultaQrCodeResponse
    {
        public Calendario calendario { get; set; }
        public string chave { get; set; }
        public Devedor devedor { get; set; }
        public Loc loc { get; set; }
        public string pixCopiaECola { get; set; }
        public Recebedor recebedor { get; set; }
        public int revisao { get; set; }
        public string solicitacaoPagador { get; set; }
        public string status { get; set; }
        public string txid { get; set; }
        public Valor valor { get; set; }
    }

    public class Calendario
    {
        public DateTime criacao { get; set; }
        public string dataDeVencimento { get; set; }
        public int validadeAposVencimento { get; set; }
    }

    public class Devedor
    {
        public string cep { get; set; }
        public string cidade { get; set; }
        public string cpf { get; set; }
        public string logradouro { get; set; }
        public string nome { get; set; }
        public string uf { get; set; }
    }

    public class Loc
    {
        public DateTime criacao { get; set; }
        public int id { get; set; }
        public string location { get; set; }
        public string tipoCob { get; set; }
    }

    public class Recebedor
    {
        public string cep { get; set; }
        public string cidade { get; set; }
        public string cnpj { get; set; }
        public string logradouro { get; set; }
        public string nome { get; set; }
        public string nomeFantasia { get; set; }
        public string uf { get; set; }
    }

    public class Valor
    {
        public string original { get; set; }
    }

}
