using System;

namespace MasterServicePro.Models
{
    public class ConfiguracaoImpressao
    {
        public int Id { get; set; }
        public string NomeLoja { get; set; }
        public string Cnpj { get; set; }
        public string Contato { get; set; }
        public string Endereco { get; set; }
        public string TermosOS { get; set; }
    }
}
