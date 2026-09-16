using System;
using System.Collections.Generic;

namespace MasterServicePro.Models.Fiscal
{
    public class VendaFiscalDTO
    {
        public int IdVenda { get; set; }
        public DateTime DataEmissao { get; set; }
        public ClienteFiscalDTO Cliente { get; set; }
        public List<ItemFiscalDTO> Itens { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public string FormaPagamento { get; set; } // Dinheiro, CartaoCredito, PIX, etc
    }

    public class ClienteFiscalDTO
    {
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Email { get; set; }
        public EnderecoFiscalDTO Endereco { get; set; }
    }

    public class EnderecoFiscalDTO
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
    }

    public class ItemFiscalDTO
    {
        public string CodigoProduto { get; set; }
        public string Descricao { get; set; }
        public string NCM { get; set; }
        public string CFOP { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public string UnidadeMedida { get; set; }
    }
}
