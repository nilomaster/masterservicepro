using System;

namespace MasterServicePro.Models
{
    public class ContaReceber
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string ClienteNome { get; set; }
        public string Descricao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorRestante { get; set; }
        public DateTime DataLancamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public string Status { get; set; } // "Pendente", "Pago", "Atrasado"

        public ContaReceber()
        {
            DataLancamento = DateTime.Now;
            DataVencimento = DateTime.Now.AddDays(30); // Padrão: 30 dias para pagar
            Status = "Pendente";
            ValorPago = 0;
        }
    }
}
