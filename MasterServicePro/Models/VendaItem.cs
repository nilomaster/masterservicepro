using System;

namespace MasterServicePro.Models
{
    public class VendaItem
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Total { get { return PrecoUnitario * Quantidade; } }

        public VendaItem() { }

        public VendaItem(Produto p, decimal qtd = 1)
        {
            IdProduto = p.Id;
            Nome = p.Nome;
            PrecoUnitario = p.PrecoVenda;
            Quantidade = qtd;
        }
    }
}
