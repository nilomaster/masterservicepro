using System;

namespace MasterServicePro.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Telefone { get; set; }
        public string WhatsApp { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public string Historico { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public decimal Saldo { get; set; }

        public Cliente()
        {
            DataCadastro = DateTime.Now;
            DataAtualizacao = DateTime.Now;
            Ativo = true;
        }
    }
}
