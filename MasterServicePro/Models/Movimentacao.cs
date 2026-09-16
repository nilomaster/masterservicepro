using System;

namespace MasterServicePro.Models
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public int IdCaixa { get; set; }
        public string Tipo { get; set; } // "Entrada", "Saída"
        public string Categoria { get; set; } // "Venda PDV", "O.S.", "Suprimento", "Sangria"
        public string Subcategoria { get; set; }
        public decimal Valor { get; set; }
        public string FormaPagamento { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }

        public Movimentacao()
        {
            Data = DateTime.Now;
        }
    }
}
