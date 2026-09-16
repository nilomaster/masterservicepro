using System;

namespace MasterServicePro.Models
{
    public class EntradaMercadoriaItem
    {
        public int Id { get; set; }
        public int IdEntrada { get; set; }
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal SubTotal { get; set; }
    }
}
