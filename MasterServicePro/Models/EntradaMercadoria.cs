using System;
using System.Collections.Generic;

namespace MasterServicePro.Models
{
    public class EntradaMercadoria
    {
        public int Id { get; set; }
        public int? IdFornecedor { get; set; }
        public string NumeroNota { get; set; }
        public DateTime DataEntrada { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public List<EntradaMercadoriaItem> Itens { get; set; }

        public EntradaMercadoria()
        {
            DataEntrada = DateTime.Now;
            ValorTotal = 0;
            Itens = new List<EntradaMercadoriaItem>();
        }
    }
}
