using System;

namespace MasterServicePro.Models
{
    public class ConfiguracaoWhatsApp
    {
        public int Id { get; set; }
        public bool UsarAPI { get; set; }
        public string ApiUrl { get; set; }
        public string ApiToken { get; set; }
        public string Instancia { get; set; }
        public string TemplateAbertura { get; set; }
        public string TemplateFinalizado { get; set; }
        public string TemplateAtualizacao { get; set; }
        public string NomeAdministrador { get; set; }
        public string TelefoneAdministrador { get; set; }
    }
}
