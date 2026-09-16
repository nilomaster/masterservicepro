using System;

namespace MasterServicePro.Models
{
    public class OrdemServicoItem
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal CustoAdicional { get; set; }
    }
}
