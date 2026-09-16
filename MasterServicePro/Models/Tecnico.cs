using System;

namespace MasterServicePro.Models
{
    public class Tecnico
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Especialidade { get; set; }
        public string Status { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }

        public Tecnico()
        {
            DataCadastro = DateTime.Now;
            DataAtualizacao = DateTime.Now;
            Status = "Disponível";
            Ativo = true;
        }
    }
}
