using System;

namespace MasterServicePro.Models
{
    public class Caixa
    {
        public int Id { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public decimal ValorAbertura { get; set; }
        public decimal? ValorFechamento { get; set; }
        public string Status { get; set; } // "Aberto", "Fechado"
        public string Observacao { get; set; }

        public Caixa()
        {
            DataAbertura = DateTime.Now;
            Status = "Aberto";
        }
    }
}
