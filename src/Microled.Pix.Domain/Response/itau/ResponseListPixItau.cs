namespace Microled.Pix.Domain.Response.itau.lista
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;


    public class ResponseListPixItau
    {
        public Parametros parametros { get; set; }
        public Cob[] cobs { get; set; }
    }

    public class Parametros
    {
        public DateTime? inicio { get; set; }
        public DateTime? fim { get; set; }
        public Paginacao paginacao { get; set; }
    }

    public class Paginacao
    {
        public int? paginaAtual { get; set; }
        public int? itensPorPagina { get; set; }
        public int? quantidadeDePaginas { get; set; }
        public int? quantidadeTotalDeItens { get; set; }
    }

    public class Cob
    {
        public Calendario calendario { get; set; }
        public Devedor devedor { get; set; }
        public Loc loc { get; set; }
        public Valor valor { get; set; }
        public string? chave { get; set; }
        public string? txid { get; set; }
        public int? revisao { get; set; }
        public Recebedor recebedor { get; set; }
        public string? status { get; set; }
        public string? pixCopiaECola { get; set; }
        public Pix[] pix { get; set; }
    }

    public class Calendario
    {
        public DateTime criacao { get; set; }
        public string? dataDeVencimento { get; set; }
        public int? validadeAposVencimento { get; set; }
    }

    public class Devedor
    {
        public string? cnpj { get; set; }
        public string? nome { get; set; }
        public string? cpf { get; set; }
    }

    public class Loc
    {
        public string id { get; set; }
        public string? location { get; set; }
        public string? tipoCob { get; set; }
        public DateTime? criacao { get; set; }
    }

    public class Valor
    {
        public string? original { get; set; }
    }

    public class Recebedor
    {
        public string? cnpj { get; set; }
        public string? nome { get; set; }
        public string? nomeFantasia { get; set; }
        public string? logradouro { get; set; }
        public string? cidade { get; set; }
        public string? uf { get; set; }
        public string? cep { get; set; }
    }

    public class Pix
    {
        public string? endToEndId { get; set; }
        public string? txid { get; set; }
        public string? valor { get; set; }
        public DateTime? horario { get; set; }
        public Componentesvalor componentesValor { get; set; }
    }

    public class Componentesvalor
    {
        public Original original { get; set; }
    }

    public class Original
    {
        public string? valor { get; set; }
    }


}
