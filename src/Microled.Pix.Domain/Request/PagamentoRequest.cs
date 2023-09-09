using Flunt.Notifications;
using Flunt.Validations;

namespace Microled.Pix.Domain.Request
{
    public class PagamentoRequest : Notifiable<Notification>
    {
        public PagamentoRequest(int id_Empresa, int id_Processo, string numero_Titulo, string devedor_Nome, string devedor_CPF, 
            string devedor_CNPJ, decimal valor, string data_Hora_Expiracao_Pagamento, string solicitacao_Pagador)
        {
            Id_Empresa = id_Empresa;
            Id_Processo = id_Processo;
            Numero_Titulo = numero_Titulo;
            Devedor_Nome = devedor_Nome;
            Devedor_CPF = devedor_CPF;
            Devedor_CNPJ = devedor_CNPJ;
            Valor = valor;
            Data_Hora_Expiracao_Pagamento = data_Hora_Expiracao_Pagamento;
            Solicitacao_Pagador = solicitacao_Pagador;
            AddNotifications(new Contract<PagamentoRequest>()
                .IsNotNullOrEmpty(numero_Titulo, "Numero do Titulo", "Numero do Titulo não pode estar vazio!")
                .IsNotNullOrEmpty(Devedor_CPF, "CPF do Devedor","CPF do devedor não pode estar vazio!")
                .IsNotNullOrEmpty(Devedor_Nome, "Nome do Devedor","Nome do devedor não pode estar vazio!")
                );
        }

        public int Id_Empresa { get; set; }
        public int Id_Processo { get; set; }
        public string Numero_Titulo { get; set; } 
        public string Devedor_Nome { get; set; }
        public string Devedor_CPF { get; set; }
        public string Devedor_CNPJ { get; set; }
        public decimal Valor { get; set; }
        public string Data_Hora_Expiracao_Pagamento { get; set; } 
        public string Solicitacao_Pagador { get; set; } 
        //public string[] Informacoes_Adicionais { get; set; } 
        public List<string> Emails_Aviso_Pagamento { get; set; } 

    }
}
