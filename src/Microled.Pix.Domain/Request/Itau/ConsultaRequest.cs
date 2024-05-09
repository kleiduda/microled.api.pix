using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microled.Pix.Domain.Request.Itau
{
    public class ConsultaRequest
    {
        //string idPagamento, 
        //string token
        public int IdPagamento { get; set; }
        public string Token { get; set; }
    }
}
