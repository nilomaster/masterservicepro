using System;

namespace MasterServicePro.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string CodigoInterno { get; set; }
        public string CodigoBarras { get; set; }
        public string Nome { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdFornecedor { get; set; }
        public string FornecedorNome { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Descricao { get; set; }
        public string Garantia { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal Margem { get; set; }
        public int Estoque { get; set; }
        public int EstoqueMinimo { get; set; }
        public string ImagemUrl { get; set; }
        public string Status { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public string CategoriaNome { get; set; }

        // Campos Fiscais Necessários para NFe/NFCe
        public string NCM { get; set; }
        public string CFOP { get; set; }
        public string CSOSN { get; set; } // Ou CST dependendo do regime tributário
        public string UnidadeMedida { get; set; } // UN, KG, LT, CX
        public decimal? PesoBruto { get; set; }
        public decimal? PesoLiquido { get; set; }

        public Produto()
        {
            DataCadastro = DateTime.Now;
            DataAtualizacao = DateTime.Now;
            Ativo = true;
            Estoque = 0;
            PrecoCusto = 0;
            PrecoVenda = 0;
        }
    }
}
