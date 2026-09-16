using System;
using System.Collections.Generic;

namespace MasterServicePro.Models
{
    public class OrdemServico
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int? IdTecnico { get; set; }
        public string NumeroOS { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string IMEI { get; set; }
        public string ClienteFinal { get; set; }
        public string Defeito { get; set; }
        public string Servico { get; set; }
        public string LaudoTecnico { get; set; }
        public decimal ValorPecas { get; set; }
        public decimal ValorServico { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; }
        public string Checklist { get; set; }
        public bool Faturado { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime? DataEntrega { get; set; }
        public List<OrdemServicoItem> Itens { get; set; }

        public OrdemServico()
        {
            DataAbertura = DateTime.Now;
            DataAtualizacao = DateTime.Now;
            Status = "Pendente";
            Itens = new List<OrdemServicoItem>();
        }
    }
}
